using ChatApplicationModels.Dtos;
using SignalRBlazorChatApp.Models;

namespace SignalRBlazorChatApp.HttpMethods
{
	public interface IPrivateGroupMessagesApiService
	{
		Task<ApiResponse<PrivateGroupMessageDto>> DeleteMessage(Guid messageId, string jsonWebToken);
		Task<ApiResponse<List<PrivateGroupMessageDto>>> GetMessagesByGroupId(int groupId, int numberMessagesToSkip, string jsonWebToken);
		Task<ApiResponse<PrivateGroupMessageDto>> PostNewMessage(CreatePrivateGroupMessageDto createDto, string jsonWebToken);
		Task<ApiResponse<PrivateGroupMessageDto>> UpdateMessage(ModifyPrivateGroupMessageDto modifyDto, string jsonWebToken);
	}
}