using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using SignalRBlazorChatApp.Helpers;
using SignalRBlazorChatApp.HttpMethods;
using Microsoft.AspNetCore.Authorization;
using SignalRBlazorChatApp.Models;
using SignalRBlazorChatApp.Models.Dtos;
using MudBlazor;

namespace SignalRBlazorChatApp.Pages
{
	[Authorize]
	public partial class PublicGroupMessages
	{
		[Parameter] public string GroupId { get; set; } = string.Empty;

		[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
		[Inject] private IJwtGenerator JwtGenerator { get; set; }
		[Inject] private IPublicChatGroupsApiService PublicChatGroupsApiService { get; set; }
		[Inject] ISnackbar Snackbar { get; set; }

		private ApiResponse<List<PublicGroupMessageDto>>? apiResponse;
		private List<PublicGroupMessageDto> _listMessagesDto;
		private string userId = string.Empty;

		protected override async Task OnInitializedAsync()
		{
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

			userId = GetUserId(authState) ?? string.Empty;

			string jsonWebToken = GenerateJwt(authState);
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

		private List<PublicGroupMessageDto> GetList(List<PublicGroupMessageDto> data)
		{
			List<PublicGroupMessageDto> newList = new();
			newList.AddRange(data);
			return newList;
		}
	}
}
