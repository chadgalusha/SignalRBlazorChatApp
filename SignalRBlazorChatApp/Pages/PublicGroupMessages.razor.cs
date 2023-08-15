using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using SignalRBlazorChatApp.Helpers;
using SignalRBlazorChatApp.HttpMethods;
using Microsoft.AspNetCore.Authorization;
using SignalRBlazorChatApp.Models;
using SignalRBlazorChatApp.Models.Dtos;
using MudBlazor;
using Microsoft.AspNetCore.SignalR.Client;
using Humanizer;

namespace SignalRBlazorChatApp.Pages
{
	[Authorize]
	public partial class PublicGroupMessages
	{
		[Parameter] public string GroupId { get; set; } = string.Empty;

		[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
		[Inject] private IJwtGenerator JwtGenerator { get; set; }
		[Inject] private IPublicGroupMessagesApiService PublicGroupMessagesApiService { get; set; }
		[Inject] ISnackbar Snackbar { get; set; }
		[Inject] IHubConnectors HubConnector { get; set; }

		private ApiResponse<List<PublicGroupMessageDto>>? initialApiResponse;
		private List<PublicGroupMessageDto> _listMessagesDto;
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

			await StartSignalR();
		}

		private async Task TestSignalRAdd()
		{
			_sendDto = new()
			{
				PublicMessageId = Guid.NewGuid(),
				ChatGroupId = 1,
				UserId = Guid.NewGuid().ToString(),
				Text = "This is a test Add"
			};
			try
			{
				await SendNewMessage();
			}
			catch (Exception e)
			{
				Snackbar.Add(e.Message, Severity.Error);
			}
			
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
				// send to SignalR hub
			}

			NewText = string.Empty;
		}

		#region SignalR Methods

		private async Task StartSignalR()
		{
			_hubConnection = HubConnector.PublicGroupMessagesConnect();

			_hubConnection!.On<PublicGroupMessageDto>("NewMessage", (dto) =>
			{
				Snackbar.Add(dto.Text);
				InvokeAsync(StateHasChanged);
			});

			_hubConnection!.On<PublicGroupMessageDto>("EditMessage", (dto) =>
			{
				Snackbar.Add(dto.Text);
				InvokeAsync(StateHasChanged);
			});

			_hubConnection!.On<int, Guid>("DeleteMessage", (groupId, deleteMessageId) =>
			{
				Snackbar.Add($"delete test: {deleteMessageId}");
				InvokeAsync(StateHasChanged);
			});
			await _hubConnection!.StartAsync();
		}

		private async Task SendNewMessage()
		{
			if (_hubConnection is not null)
			{
				await _hubConnection.SendAsync("SendMessage", _sendDto);
			}
		}

		private async Task SendEditMessage()
		{
			if (_hubConnection is not null)
			{
				await _hubConnection.SendAsync("EditMessage", _sendDto);
			}
		}

		private async Task SendDeleteMessage()
		{
			if (_hubConnection is not null)
			{
				await _hubConnection.SendAsync("DeleteMessage", Convert.ToInt32(GroupId), _deleteMessageId);
			}
		}

		#endregion
	}
}
