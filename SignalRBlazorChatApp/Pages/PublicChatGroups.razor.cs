﻿using ChatApplicationModels.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using SignalRBlazorChatApp.Helpers;
using SignalRBlazorChatApp.HttpMethods;
using SignalRBlazorChatApp.Models;

namespace SignalRBlazorChatApp.Pages
{
	[Authorize]
	public partial class PublicChatGroups
	{
		[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
		[Inject] private IJwtGenerator JwtGenerator { get; set; } = default!;
		[Inject] private IPublicChatGroupsApiService PublicChatGroupsApiService { get; set; } = default!;
		[Inject] NavigationManager NavigationManager { get; set; } = default!;
		[Inject] IHubConnectors HubConnector { get; set; } = default!;
		[Inject] ISnackbar Snackbar { get; set; } = default!;
		[Inject] IDialogService DialogService { get; set; } = default!;

		private ApiResponse<List<PublicChatGroupsDto>>? initialApiResponse;
		private List<PublicChatGroupsDto>? _listPublicChatGroupsDto;
		private string UserId = string.Empty;

		// Form variables
		private CreatePublicChatGroupDto createDto;
		private PublicChatGroupsDto editDto;
		private bool ShowNewPopup = false;
		private bool ShowEditPopup = false;

		// SignalR variables
		private HubConnection? _hubConnection;

		protected override async Task OnInitializedAsync()
		{
			string jsonWebToken = await LoadUserData();

			initialApiResponse = await PublicChatGroupsApiService.GetPublicChatGroupsAsync(jsonWebToken);

			_listPublicChatGroupsDto = GetList(initialApiResponse.Data!);
		}

		private string? GetUserId(AuthenticationState authState)
		{
            var user = authState.User;
            var userId = user.FindFirst(c => c.Type.Contains("nameidentifier"))?.Value;
			return userId;
        }

		private async Task<string> LoadUserData()
		{
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			UserId = GetUserId(authState) ?? string.Empty;
			return GenerateJwt(authState);
		}

		private string GenerateJwt(AuthenticationState authState)
		{
			return JwtGenerator.GetJwtToken(authState);
		}

		public async Task<string> RegenerateJWT()
		{
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			return GenerateJwt(authState);
		}

		private List<PublicChatGroupsDto> GetList(List<PublicChatGroupsDto> data)
		{
			List<PublicChatGroupsDto> newList = new();
			newList.AddRange(data);
			return newList;
		}

		private bool UserIdMatch(string userId, string compareId) => userId == compareId;

		private void RedirectToGroupMessages(int groupId, string groupName)
		{
			NavigationManager.NavigateTo($"PublicGroupMessages/{groupId}/{groupName}");
		}

		#region CRUD -R METHODS

		private async Task PostNewGroup()
		{
			CancelNew();

			var jsonWebToken = await RegenerateJWT();

			ApiResponse<PublicChatGroupsDto> apiResponse = new();

			try
			{
				apiResponse = await PublicChatGroupsApiService.PostNewGroup(createDto, jsonWebToken);

				if (!apiResponse.Success)
					Snackbar.Add(apiResponse.Message, Severity.Error);
				if (apiResponse.Success && apiResponse.Data != null)
				{
					_listPublicChatGroupsDto!.Add(apiResponse.Data);
				}
			}
			catch (Exception ex)
			{
				Snackbar.Add(ex.Message, Severity.Error);
			}

			createDto = new();
		}

		void ShowNewForm()
		{
			CancelEdit();

			createDto = new()
			{
				GroupOwnerUserId = UserId,
				ChatGroupName = string.Empty
			};

			ShowNewPopup = true;
		}

		void CancelNew()
		{
			ShowNewPopup = false;
		}

		void ShowEditForm(PublicChatGroupsDto dto)
		{
			CancelNew();

			editDto = new()
			{
				ChatGroupId		 = dto.ChatGroupId,
				ChatGroupName	 = dto.ChatGroupName,
				GroupCreated	 = dto.GroupCreated,
				GroupOwnerUserId = dto.GroupOwnerUserId,
				UserName		 = dto.UserName
			};

			ShowEditPopup = true;
		}

		void CancelEdit()
		{
			ShowEditPopup = false;
		}

		async Task EditGroup()
		{
			ShowEditPopup = false;

			ModifyPublicChatGroupDto modifyDto = new()
			{
				ChatGroupId = editDto.ChatGroupId,
				ChatGroupName = editDto.ChatGroupName
			};

			string jsonWebToken = await RegenerateJWT();

			ApiResponse<PublicChatGroupsDto> apiResponse = new();

			try
			{
				apiResponse = await PublicChatGroupsApiService.UpdateGroup(modifyDto, jsonWebToken);

				if (!apiResponse.Success)
					Snackbar.Add(apiResponse.Message, Severity.Error);
				if (apiResponse.Success && apiResponse.Data != null)
				{
					var group = _listPublicChatGroupsDto!.Single(id => id.ChatGroupId == apiResponse.Data.ChatGroupId);
					group.ChatGroupName = apiResponse.Data.ChatGroupName;
				}
			}
			catch (Exception ex)
			{
				Snackbar.Add(ex.Message, Severity.Error);
			}

			editDto = new();
		}

		async Task DeleteConfirm(PublicChatGroupsDto dto)
		{
			bool? confirmed = await DialogService.ShowMessageBox(
				"Warning",
				$"Permanently Delete Chat Group: {dto.ChatGroupName}? Doing so will also delete any messages in the group.",
				yesText: "Delete",
				cancelText: "Cancel"
				);

			if (confirmed is true) { await DeleteGroup(dto.ChatGroupId); }
		}

		async Task DeleteGroup(int groupId)
		{
			string jsonWebToken = await RegenerateJWT();

			ApiResponse<PublicChatGroupsDto> apiResponse = new();

			try
			{
				apiResponse = await PublicChatGroupsApiService.DeleteGroup(groupId, jsonWebToken);

				if (!apiResponse.Success)
					Snackbar.Add(apiResponse.Message, Severity.Error);
				else
				{
					var groupToRemove = _listPublicChatGroupsDto!.Single(id => id.ChatGroupId == groupId);
					await SendSignalRDeleteMessage(groupToRemove.ChatGroupId.ToString());
					_listPublicChatGroupsDto!.Remove(groupToRemove);
				}
			}
			catch (Exception ex)
			{
				Snackbar.Add(ex.Message, Severity.Error);
			}
		}

		#endregion
		#region SignalR methods
		
		// If a user is in the chat group being deleted, re-route to public groups page
		private async Task SendSignalRDeleteMessage(string groupId)
		{
			await StartSignalR();
			await SendGroupDeleted(groupId);
			await StopSignalR();
		}

		private async Task StartSignalR()
		{
			_hubConnection = HubConnector.PublicGroupMessagesConnect();
			await _hubConnection!.StartAsync();
		}

		private async Task StopSignalR()
		{
			if (_hubConnection is not null)
			{
				await _hubConnection.StopAsync();
			}
		}

		private async Task SendGroupDeleted(string groupId)
		{
			if (_hubConnection is not null)
			{
				await _hubConnection.SendAsync("ChatGroupDeleted", groupId);
			}
		}

		#endregion
	}
}
