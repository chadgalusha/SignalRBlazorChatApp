using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using SignalRBlazorChatApp.Helpers;
using SignalRBlazorChatApp.HttpMethods;
using Microsoft.AspNetCore.Authorization;
using SignalRBlazorChatApp.Models;
using SignalRBlazorChatApp.Models.Dtos;
using MudBlazor;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRBlazorChatApp.Pages
{
	[Authorize]
	public partial class PublicGroupMessages
	{
		[Parameter] public string GroupId { get; set; } = string.Empty;

		[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
		[Inject] private IJwtGenerator JwtGenerator { get; set; } = default!;
		[Inject] private IPublicGroupMessagesApiService PublicGroupMessagesApiService { get; set; } = default!;
		[Inject] ISnackbar Snackbar { get; set; } = default!;
		[Inject] IHubConnectors HubConnector { get; set; } = default!;

		private ApiResponse<List<PublicGroupMessageDto>>? initialApiResponse;
		private List<PublicGroupMessageDto> _listMessagesDto;
		private string ChatGroupName = string.Empty;
		private string userId = string.Empty;
		// SignalR variables
		private HubConnection? _hubConnection;
		private PublicGroupMessageDto _sendDto;
		private Guid _deleteMessageId;
		// Form variables
		private string NewText { get; set; } = string.Empty;

		protected override async Task OnInitializedAsync()
		{
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			userId = GetUserId(authState) ?? string.Empty;
			string jsonWebToken = GenerateJwt(authState);

			initialApiResponse = await PublicGroupMessagesApiService.GetMessagesByGroupId(Convert.ToInt32(GroupId), 0, jsonWebToken);
			_listMessagesDto = GetInitialList(initialApiResponse.Data!);
			ChatGroupName = _listMessagesDto.First().ChatGroupName;

			await StartSignalR();
		}

		private async Task TestSignalREdit()
		{
			_sendDto = new()
			{
				PublicMessageId = Guid.NewGuid(),
				ChatGroupId = 1,
				UserId = Guid.NewGuid().ToString(),
				Text = "This is a test Edit"
			};
			try
			{
				await SendEditMessage();
			}
			catch (Exception e)
			{
				Snackbar.Add(e.Message, Severity.Error);
			}
		}

		private async Task TestSignalRDelete()
		{
			try
			{
				_deleteMessageId = Guid.NewGuid();
				await SendDeleteMessage();
			}
			catch (Exception e)
			{
				Snackbar.Add(e.Message, Severity.Error);
			}
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

		private List<PublicGroupMessageDto> GetInitialList(List<PublicGroupMessageDto> data)
		{
			List<PublicGroupMessageDto> newList = new();
			newList.AddRange(data);
			newList.Reverse();
			return newList;
		}

		private async Task PostNewMessage(string userId, string groupId, string text)
		{
			CreatePublicGroupMessageDto createDto = new()
			{
				UserId		= userId,
				ChatGroupId = Convert.ToInt32(groupId),
				Text	    = text
			};

			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			string jsonWebToken = GenerateJwt(authState);

			ApiResponse<PublicGroupMessageDto> apiResponse = new();

			try
			{
				apiResponse = await PublicGroupMessagesApiService.PostNewMessage(createDto, jsonWebToken);
			}
			catch (Exception ex)
			{
				Snackbar.Add(ex.Message, Severity.Error);
			}

			if (apiResponse.Success == false)
			{
				Snackbar.Add(apiResponse.Message, Severity.Error);
			}
			else
			{
				if (apiResponse.Data != null)
				{
					await SendNewMessage(apiResponse.Data);
				}
			}

			NewText = string.Empty;
		}

		#region SignalR Methods

		private async Task StartSignalR()
		{
			_hubConnection = HubConnector.PublicGroupMessagesConnect();
			HubListenerMethods(_hubConnection!);
			await _hubConnection!.StartAsync();
			await _hubConnection.SendAsync("AddToGroup", GroupId);
		}

		private void HubListenerMethods(HubConnection hubConnection)
		{
			hubConnection!.On<PublicGroupMessageDto>("ReceiveAdd", (dto) =>
			{
				_listMessagesDto.Add(dto);
				InvokeAsync(StateHasChanged);
			});

			hubConnection!.On<PublicGroupMessageDto>("ReceiveEdit", (dto) =>
			{
				Snackbar.Add(dto.Text);
				InvokeAsync(StateHasChanged);
			});

			hubConnection!.On<Guid>("ReceiveDelete", (deleteMessageId) =>
			{
				Snackbar.Add($"delete test: {deleteMessageId}");
				var dtoToDelete = _listMessagesDto.SingleOrDefault(id => id.PublicMessageId == deleteMessageId);
				if (dtoToDelete != null)
				{
					_listMessagesDto.Remove(dtoToDelete);
				}
				InvokeAsync(StateHasChanged);
			});

			hubConnection!.On<string>("ReceiveGroupMessage", (message) =>
			{
				Snackbar.Add($"Group test: {message}");
				InvokeAsync(StateHasChanged);
			});
		}

		private async Task SendNewMessage(PublicGroupMessageDto dto)
		{
			if (_hubConnection is not null)
			{
				await _hubConnection.SendAsync("SendGroupMessageAdd", GroupId, dto);
			}
		}

		private async Task SendEditMessage()
		{
			if (_hubConnection is not null)
			{
				await _hubConnection.SendAsync("SendGroupMessageEdit", GroupId, _sendDto);
			}
		}

		private async Task SendDeleteMessage()
		{
			if (_hubConnection is not null)
			{
				await _hubConnection.SendAsync("SendGroupMessageDelete", GroupId, _deleteMessageId);
			}
		}

		private async Task SendGroupMessage()
		{
			if (_hubConnection is not null)
			{
				await _hubConnection.SendAsync("TestGroupMessage", GroupId);
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
