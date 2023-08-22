using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SignalRBlazorChatApp.Helpers;
using SignalRBlazorChatApp.HttpMethods;
using SignalRBlazorChatApp.Models;
using SignalRBlazorChatApp.Models.Dtos;

namespace SignalRBlazorChatApp.Pages
{
	[Authorize]
	public partial class PublicChatGroups
	{
		[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
		[Inject] private IJwtGenerator JwtGenerator { get; set; } = default!;
		[Inject] private IPublicChatGroupsApiService PublicChatGroupsApiService { get; set; } = default!;
		[Inject] NavigationManager NavigationManager { get; set; } = default!;

		private ApiResponse<List<PublicChatGroupsDto>>? apiResponse;
		private List<PublicChatGroupsDto>? _listPublicChatGroupsDto;
		private string userId = string.Empty;

		protected override async Task OnInitializedAsync()
		{
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

			userId = GetUserId(authState) ?? string.Empty;
			
			string jsonWebToken = GenerateJwt(authState);

			apiResponse = await PublicChatGroupsApiService.GetPublicChatGroupsAsync(jsonWebToken);

			_listPublicChatGroupsDto = GetList(apiResponse.Data!);
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

		private List<PublicChatGroupsDto> GetList(List<PublicChatGroupsDto> data)
		{
			List<PublicChatGroupsDto> newList = new();
			newList.AddRange(data);
			return newList;
		}

		private bool UserIdMatch(string userId, string compareId) => userId == compareId;

		private void RedirectToGroupMessages(int groupId)
		{
			NavigationManager.NavigateTo($"PublicGroupMessages/{groupId}");
		}

		#region CRUD -R METHODS

		private async Task PostNewGroup()
		{

		}

		#endregion
	}
}
