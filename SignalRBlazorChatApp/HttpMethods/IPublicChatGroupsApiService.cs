using SignalRBlazorChatApp.Models;
using SignalRBlazorChatApp.Models.Dtos;

namespace SignalRBlazorChatApp.HttpMethods
{
	public interface IPublicChatGroupsApiService
	{
		Task<ApiResponse<PublicChatGroupsDto>> DeleteMessage(Guid groupId, string jsonWebToken);
		Task<ApiResponse<List<PublicChatGroupsDto>>> GetPublicChatGroupsAsync(string jsonWebToken);
		Task<ApiResponse<PublicChatGroupsDto>> PostNewGroup(CreatePublicChatGroupDto createDto, string jsonWebToken);
		Task<ApiResponse<PublicChatGroupsDto>> UpdateGroup(ModifyPublicChatGroupDto modifyDto, string jsonWebToken);
	}
}