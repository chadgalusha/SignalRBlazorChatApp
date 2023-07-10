using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public interface IPublicMessagesService
    {
        Task<ApiResponse<PublicMessageDto>> AddAsync(PublicMessageDto messageDto);
        Task<ApiResponse<PublicMessageDto>> DeleteAsync(PublicMessageDto dtoMessage);
        Task<ApiResponse<List<PublicMessageDto>>> GetListByGroupIdAsync(int groupId, int numberItemsToSkip);
        Task<ApiResponse<PublicMessageDto>> GetByMessageIdAsync(Guid messageId);
        Task<ApiResponse<List<PublicMessageDto>>> GetViewListByUserIdAsync(Guid userId, int numberItemsToSkip);
        Task<ApiResponse<PublicMessageDto>> ModifyAsync(PublicMessageDto dtoMessage);
    }
}