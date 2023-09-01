using ChatApplicationModels.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using SignalRBlazorChatApp.Helpers;
using SignalRBlazorChatApp.Models;
using SignalRBlazorChatApp.Services;

namespace SignalRBlazorChatApp.Pages
{
    [Authorize]
	public partial class PrivateGroupMessages
	{
		[Parameter] public string GroupId { get; set; } = string.Empty;
		[Parameter] public string ChatGroupName { get; set; } = string.Empty;

		[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
		[Inject] private IJwtGenerator JwtGenerator { get; set; } = default!;
		[Inject] private IPrivateGroupMessagesApiService PrivateGroupMessagesApiService { get; set; } = default!;
		[Inject] ISnackbar Snackbar { get; set; } = default!;
		[Inject] IDialogService DialogService { get; set; } = default!;
		[Inject] IHubConnectors HubConnector { get; set; } = default!;
		[Inject] NavigationManager NavigationManager { get; set; } = default!;

		// Javascript functionality
		[Inject] IJSRuntime JSRuntime { get; set; } = default!;
		private Task<IJSObjectReference> _module;
		private Task<IJSObjectReference> Module => _module ??= JSRuntime
			.InvokeAsync<IJSObjectReference>("import", "./js/chatfunctions.js").AsTask();

		private ApiResponse<List<PrivateGroupMessageDto>>? initialApiResponse;
		private List<PrivateGroupMessageDto> _listMessagesDto;

		private string userId = string.Empty;
		bool ShowEditPopup = false;

		// Signal R variables
		private HubConnection? _hubConnection;

		// Form variables
		private string NewText { get; set; } = string.Empty;
		private PrivateGroupMessageDto editDto;

		protected override async Task OnInitializedAsync()
		{
			string jsonWebToken = await LoadUserData();
			await LoadData(jsonWebToken);
			await StartSignalR();
		}

		protected override Task OnParametersSetAsync()
		{
			Task.Run(ScrollToBottom);
			return base.OnParametersSetAsync();
		}

		private async Task ScrollToBottom()
		{
			var module = await Module;
			await module.InvokeVoidAsync("scrollToBottom");
		}

		#region USER METHODS

		private async Task<string> LoadUserData()
		{
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			userId = GetUserId(authState) ?? string.Empty;
			return GenerateJwt(authState);
		}

		private string? GetUserId(AuthenticationState authState)
		{
			var user = authState.User;
			var userId = user.FindFirst(c => c.Type.Contains("nameidentifier"))?.Value;
			return userId;
		}

		private string GenerateJwt(AuthenticationState authState)
		{
			return JwtGenerator.GetJwtToken(authState);
		}

		private async Task<string> RegenerateJWT()
		{
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			return GenerateJwt(authState);
		}

		#endregion
		#region DATA METHODS

		private async Task LoadData(string jsonWebToken)
		{
			initialApiResponse = await PrivateGroupMessagesApiService.GetMessagesByGroupId(Convert.ToInt32(GroupId), 0, jsonWebToken);
			_listMessagesDto = GetInitialList(initialApiResponse.Data!);
		}

		private List<PrivateGroupMessageDto> GetInitialList(List<PrivateGroupMessageDto> data)
		{
			List<PrivateGroupMessageDto> newList = new();
			newList.AddRange(data);
			newList.Reverse();
			return newList;
		}

		private async Task LoadAdditionalData()
		{
			int listCount = _listMessagesDto.Count;

			string jsonWebToken = await LoadUserData();

			var apiResponse = await PrivateGroupMessagesApiService.GetMessagesByGroupId(Convert.ToInt32(GroupId), listCount, jsonWebToken);

			if (!apiResponse.Success)
				Snackbar.Add("Error loading data", Severity.Error);
			if (apiResponse.Data != null)
			{
				var additionalDataList = apiResponse.Data
					.OrderBy(d => d.MessageDateTime)
					.ToList();

				_listMessagesDto.InsertRange(0, additionalDataList);
			}
		}

		#endregion
		#region CRUD methods -R

		private async Task PostNewMessage(string userId, string groupId, string text)
		{
			CreatePrivateGroupMessageDto createDto = new()
			{
				UserId		= userId,
				ChatGroupId = Convert.ToInt32(GroupId),
				Text		= text
			};

			string jsonWebToken = await RegenerateJWT();

			ApiResponse<PrivateGroupMessageDto> apiResponse = new();

			try
			{
				apiResponse = await PrivateGroupMessagesApiService.PostNewMessage(createDto, jsonWebToken);

				if (!apiResponse.Success)
					Snackbar.Add(apiResponse.Message, Severity.Error);
				if (apiResponse.Success && apiResponse.Data != null)
					await SendNewMessage(apiResponse.Data);
			}
			catch (Exception ex)
			{
				Snackbar.Add(ex.Message, Severity.Error);
			}

			NewText = string.Empty;
		}

		void ShowEditForm(PrivateGroupMessageDto dto)
		{
			editDto = new()
			{
				PrivateMessageId = dto.PrivateMessageId,
				UserId			 = dto.UserId,
				UserName		 = dto.UserName,
				ChatGroupId		 = dto.ChatGroupId,
				ChatGroupName	 = dto.ChatGroupName,
				Text		     = dto.Text,
				MessageDateTime  = dto.MessageDateTime,
				ReplyMessageId   = dto.ReplyMessageId,
				PictureLink		 = dto.PictureLink
			};

			ShowEditPopup = true;
		}

		void CancelEdit()
		{
			ShowEditPopup = false;
		}

		private async Task EditMessage()
		{
			ShowEditPopup = false;

			ModifyPrivateGroupMessageDto modifyDto = new()
			{
				PrivateMessageId = editDto.PrivateMessageId,
				Text = editDto.Text
			};

			string jsonWebToken = await RegenerateJWT();

			ApiResponse<PrivateGroupMessageDto> apiResponse = new();

			try
			{
				apiResponse = await PrivateGroupMessagesApiService.UpdateMessage(modifyDto, jsonWebToken);

				if (!apiResponse.Success)
					Snackbar.Add(apiResponse.Message, Severity.Error);
				if (apiResponse.Success && apiResponse.Data != null)
					await SendEditMessage(apiResponse.Data);
			}
			catch (Exception ex)
			{
				Snackbar.Add(ex.Message, Severity.Error);
			}
		}

		async Task DeleteConfirm(PrivateGroupMessageDto dto)
		{
			bool? confirmed = await DialogService.ShowMessageBox(
				"Warning",
				$"Permanently Delete Your Message: {dto.Text} - from {dto.MessageDateTime.ToShortDateString()}",
				yesText: "Delete",
				cancelText: "Cancel");

			if (confirmed is true) { await DeleteMessage(dto.PrivateMessageId); }
		}

		private async Task DeleteMessage(Guid messageId)
		{
			string jsonWebToken = await RegenerateJWT();

			ApiResponse<PrivateGroupMessageDto> apiResponse = new();

			try
			{
				apiResponse = await PrivateGroupMessagesApiService.DeleteMessage(messageId, jsonWebToken);

				if (!apiResponse.Success)
					Snackbar.Add(apiResponse.Message, Severity.Error);
				else
					await SendDeleteMessage(messageId);
			}
			catch (Exception ex)
			{
				Snackbar.Add(ex.Message, Severity.Error);
			}
		}
		#endregion
		#region SignalR Methods

		private async Task StartSignalR()
		{
			_hubConnection = HubConnector.PrivateGroupMessagesConnect();
			HubListenerMethods(_hubConnection!);
			await _hubConnection!.StartAsync();
			await _hubConnection.SendAsync("AddToGroup", GroupId);
		}

		private void HubListenerMethods(HubConnection hubConnection)
		{
			hubConnection!.On<PrivateGroupMessageDto>("ReceiveAdd", (dto) =>
			{
				_listMessagesDto.Add(dto);
				InvokeAsync(StateHasChanged);
				Task.Run(() => ScrollToBottom());
			});

			hubConnection!.On<PrivateGroupMessageDto>("ReceiveEdit", (dto) =>
			{
				var dtoToEdit = _listMessagesDto.Single(id => id.PrivateMessageId == dto.PrivateMessageId);
				dtoToEdit.Text = dto.Text;
				InvokeAsync(StateHasChanged);
			});

			hubConnection!.On<Guid>("ReceiveDelete", (deleteMessageId) =>
			{
				var dtoToDelete = _listMessagesDto.SingleOrDefault(id => id.PrivateMessageId == deleteMessageId);
				if (dtoToDelete != null)
				{
					_listMessagesDto.Remove(dtoToDelete);
				}
				InvokeAsync(StateHasChanged);
			});

			hubConnection!.On<string>("GroupDeleted", (groupId) =>
			{
				if (GroupId == groupId)
				{
					NavigationManager.NavigateTo("/PrivateChatGroups");
				}
			});
		}

		private async Task SendNewMessage(PrivateGroupMessageDto dto)
		{
			if (_hubConnection is not null)
			{
				await _hubConnection.SendAsync("SendGroupMessageAdd", GroupId, dto);
			}
		}

		private async Task SendEditMessage(PrivateGroupMessageDto dto)
		{
			if (_hubConnection is not null)
			{
				await _hubConnection.SendAsync("SendGroupMessageEdit", GroupId, dto);
			}
		}

		private async Task SendDeleteMessage(Guid messageId)
		{
			if (_hubConnection is not null)
			{
				await _hubConnection.SendAsync("SendGroupMessageDelete", GroupId, messageId);
			}
		}

		#endregion

		public void Dispose()
		{
			if (_hubConnection is not null)
			{
				Task.Run(() => _hubConnection.StopAsync());
			}
		}
	}
}
