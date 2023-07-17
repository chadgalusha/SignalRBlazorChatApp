using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public interface IPublicMessagesService
    {
        Task<ApiResponse<PublicGroupMessageDto>> AddAsync(PublicGroupMessageDto messageDto);
        Task<ApiResponse<PublicGroupMessageDto>> DeleteAsync(PublicGroupMessageDto dtoMessage);
        Task<ApiResponse<List<PublicGroupMessageDto>>> GetListByGroupIdAsync(int groupId, int numberItemsToSkip);
        Task<ApiResponse<PublicGroupMessageDto>> GetByMessageIdAsync(Guid messageId);
        Task<ApiResponse<List<PublicGroupMessageDto>>> GetViewListByUserIdAsync(Guid userId, int numberItemsToSkip);
        Task<ApiResponse<PublicGroupMessageDto>> ModifyAsync(PublicGroupMessageDto dtoMessage);
        Task<bool> DeleteAllMessagesInGroupAsync(int chatGroupId);
    }
}