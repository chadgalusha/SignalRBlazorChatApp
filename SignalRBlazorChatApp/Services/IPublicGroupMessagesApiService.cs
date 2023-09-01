using ChatApplicationModels.Dtos;
using SignalRBlazorChatApp.Models;

namespace SignalRBlazorChatApp.Services
{
    public interface IPublicGroupMessagesApiService
    {
        Task<ApiResponse<PublicGroupMessageDto>> DeleteMessage(Guid messageId, string jsonWebToken);
        Task<ApiResponse<List<PublicGroupMessageDto>>> GetMessagesByGroupId(int groupId, int numberItemsToSkip, string jsonWebToken);
        Task<ApiResponse<PublicGroupMessageDto>> PostNewMessage(CreatePublicGroupMessageDto createDto, string jsonWebToken);
        Task<ApiResponse<PublicGroupMessageDto>> UpdateMessage(ModifyPublicGroupMessageDto modifyDto, string jsonWebToken);
    }
}