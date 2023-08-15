using SignalRBlazorChatApp.Models;
using SignalRBlazorChatApp.Models.Dtos;

namespace SignalRBlazorChatApp.HttpMethods
{
	public interface IPublicGroupMessagesApiService
	{
		Task<ApiResponse<List<PublicGroupMessageDto>>> GetMessagesByGroupId(int groupId, int numberItemsToSkip, string jsonWebToken);
		Task<ApiResponse<PublicGroupMessageDto>> PostNewMessage(CreatePublicGroupMessageDto createDto, string jsonWebToken);
	}
}