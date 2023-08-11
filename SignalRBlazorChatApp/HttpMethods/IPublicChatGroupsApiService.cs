using SignalRBlazorChatApp.Models;
using SignalRBlazorChatApp.Models.Dtos;

namespace SignalRBlazorChatApp.HttpMethods
{
	public interface IPublicChatGroupsApiService
	{
		Task<ApiResponse<List<PublicChatGroupsDto>>> GetPublicChatGroupsAsync(string jsonWebToken);
	}
}