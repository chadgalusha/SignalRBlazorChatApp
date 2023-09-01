using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using SignalRBlazorChatApp.Helpers;
using MudBlazor;
using SignalRBlazorChatApp.Models;
using ChatApplicationModels.Dtos;
using Microsoft.AspNetCore.SignalR.Client;
using ChatApplicationModels;
using SignalRBlazorChatApp.Services;

namespace SignalRBlazorChatApp.Pages
{
    [Authorize]
	public partial class PrivateChatGroups
	{
		[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
		[Inject] private IJwtGenerator JwtGenerator { get; set; } = default!;
		[Inject] private IPrivateChatGroupsApiService PrivateChatGroupsApiService { get; set; } = default!;
		[Inject] NavigationManager NavigationManager { get; set; } = default!;
		[Inject] IHubConnectors HubConnector { get; set; } = default!;
		[Inject] ISnackbar Snackbar { get; set; } = default!;
		[Inject] IDialogService DialogService { get; set; } = default!;

		private ApiResponse<List<PrivateChatGroupsDto>>? initialApiResponse;
		private List<PrivateChatGroupsDto>? _ListPrivateChatGroupsDto;
		private string UserId = string.Empty;

		// Form variables
		private CreatePrivateChatGroupDto createDto;
		private PrivateChatGroupsDto editDto;
		private bool ShowNewPopup = false;
		private bool ShowEditPopup = false;


		// SignalR variables
		private HubConnection? _hubConnection;

		protected override async Task OnInitializedAsync()
		{
			string jsonWebToken = await LoadUserData();

			initialApiResponse = await PrivateChatGroupsApiService.GetPrivateChatGroupsAsync(UserId, jsonWebToken);

			_ListPrivateChatGroupsDto = GetList(initialApiResponse.Data!);
		}

		#region USER METHODS

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

		#endregion
		#region DATA METHODS

		private List<PrivateChatGroupsDto> GetList(List<PrivateChatGroupsDto> data)
		{
			List<PrivateChatGroupsDto> newList = new();
			newList.AddRange(data);
			return newList;
		}

		private bool UserIdMatch(string userId, string compareId) => userId == compareId;

		private void RedirectToGroupMessages(int groupId, string groupName)
		{
			NavigationManager.NavigateTo($"PrivateGroupMessages/{groupId}/{groupName}");
		}

		#endregion
		#region CRUD METHODS

		private async Task PostNewGroup()
		{
			CancelNew();

			var jsonWebToken = await RegenerateJWT();

			ApiResponse<PrivateChatGroupsDto> apiResponse = new();

			try
			{
				apiResponse = await PrivateChatGroupsApiService.PostNewGroup(createDto, jsonWebToken);

				if (!apiResponse.Success)
					Snackbar.Add(apiResponse.Message, Severity.Error);
				if (apiResponse.Success && apiResponse.Data != null)
				{
					await AddGroupMember(apiResponse.Data.ChatGroupId, UserId, jsonWebToken);
					_ListPrivateChatGroupsDto!.Add(apiResponse.Data);
				}
			}
			catch (Exception ex)
			{
				Snackbar.Add(ex.Message, Severity.Error);
			}

			createDto = new();
		}

		private async Task AddGroupMember(int groupId, string userId, string jsonWebToken)
		{
			try
			{
				ApiResponse<PrivateGroupMembers> apiResponse = await PrivateChatGroupsApiService.PostGroupMember(
					groupId, userId, jsonWebToken);

				if (!apiResponse.Success)
					Snackbar.Add(apiResponse.Message, Severity.Error);

			}
			catch (Exception ex)
			{
				Snackbar.Add("Error adding group member", Severity.Error);
				Snackbar.Add(ex.Message, Severity.Error);
			}
		}

		private async Task EditGroup()
		{
			ShowEditPopup = false;

			ModifyPrivateChatGroupDto modifyDto = new()
			{
				ChatGroupId	  = editDto.ChatGroupId,
				ChatGroupName = editDto.ChatGroupName
			};

			string jsonWebToken = await RegenerateJWT();

			ApiResponse<PrivateChatGroupsDto> apiResponse = new();

			try
			{
				apiResponse = await PrivateChatGroupsApiService.UpdateGroup(modifyDto, jsonWebToken);

				if (!apiResponse.Success)
					Snackbar.Add(apiResponse.Message, Severity.Error);
				if (apiResponse.Success && apiResponse.Data != null)
				{
					var group = _ListPrivateChatGroupsDto!.Single(id => id.ChatGroupId == apiResponse.Data.ChatGroupId);
					group.ChatGroupName = apiResponse.Data.ChatGroupName;
				}
			}
			catch (Exception ex)
			{
				Snackbar.Add(ex.Message, Severity.Error);
			}

			editDto = new();
		}

		async Task DeleteConfirm(PrivateChatGroupsDto dto)
		{
			bool? confirmed = await DialogService.ShowMessageBox(
				"Warning",
				$"Permanently Delete Chat Group: {dto.ChatGroupName}? Doing so will also delete any messages in the group.",
				yesText: "Delete",
				cancelText: "Cancel"
				);

			if (confirmed is true) { await DeleteGroup(dto.ChatGroupId); }
		}

		private async Task DeleteGroup(int groupId)
		{
			var jsonWebToken = await RegenerateJWT();

			ApiResponse<PrivateChatGroupsDto> apiResponse = new();

			try
			{
				apiResponse = await PrivateChatGroupsApiService.DeleteGroup(groupId, jsonWebToken);

				if (!apiResponse.Success)
					Snackbar.Add(apiResponse.Message, Severity.Error);
				else
				{
					var groupToRemove = _ListPrivateChatGroupsDto!.Single(id => id.ChatGroupId == groupId);
					await SendSignalRDeleteMessage(groupToRemove.ChatGroupId.ToString());
					_ListPrivateChatGroupsDto!.Remove(groupToRemove);
				}
			}
			catch (Exception ex)
			{
				Snackbar.Add(ex.Message, Severity.Error);
			}
		}

		void ShowNewForm()
		{
			CancelEdit();

			createDto = new()
			{
				GroupOwnerUserId = UserId,
				ChatGroupName	 = string.Empty
			};

			ShowNewPopup = true;
		}

		void CancelNew()
		{
			ShowNewPopup = false;
		}

		void ShowEditForm(PrivateChatGroupsDto dto)
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

		#endregion
		#region SignalR Methods

		// re-route user in a deleted chat group to private groups page
		private async Task SendSignalRDeleteMessage(string groupId)
		{
			await StartSignalR();
			await SendGroupDeleted(groupId);
			await StopSignalR();
		}

		private async Task StartSignalR()
		{
			_hubConnection = HubConnector.PrivateGroupMessagesConnect();
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
