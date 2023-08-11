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
		[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
		[Inject] private IJwtGenerator JwtGenerator { get; set; }
		[Inject] private IPublicChatGroupsApiService PublicChatGroupsApiService { get; set; }

		private ApiResponse<List<PublicChatGroupsDto>>? apiResponse;
		private List<PublicChatGroupsDto>? _listPublicChatGroupsDto;

		protected override async Task OnInitializedAsync()
		{
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

			(string, string) userIdAndRole = UserProcessor.GetUserIdAndRole(authState);
			var jsonWebToken = JwtGenerator.GetJwtToken(userIdAndRole.Item1, userIdAndRole.Item2);

			apiResponse = await PublicChatGroupsApiService.GetPublicChatGroupsAsync(jsonWebToken);

			_listPublicChatGroupsDto = GetList(apiResponse.Data);
		}

		private async Task<(string, string)> GetAuthenticatedUserIdAndRole()
		{
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			return UserProcessor.GetUserIdAndRole(authState);
        }

		private List<PublicChatGroupsDto> GetList(List<PublicChatGroupsDto> data)
		{
			List<PublicChatGroupsDto> newList = new();
			List<PublicChatGroupsDto> dataList = data;
			foreach (var item in dataList)
			{
				PublicChatGroupsDto dto = new()
				{
					ChatGroupId = item.ChatGroupId,
					ChatGroupName = item.ChatGroupName,
					GroupCreated = item.GroupCreated,
					GroupOwnerUserId = item.GroupOwnerUserId,
					UserName = item.UserName
				};
				newList.Add(dto);
			}

			return newList;
		}
	}
}
