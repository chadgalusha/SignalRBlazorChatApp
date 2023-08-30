using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public interface IPublicGroupMessagesService
    {
        Task<ApiResponse<PublicGroupMessageDto>> AddAsync(CreatePublicGroupMessageDto messageDto);
        Task<ApiResponse<PublicGroupMessageDto>> DeleteAsync(Guid messageId, string UserId);
        Task<ApiResponse<List<PublicGroupMessageDto>>> GetListByGroupIdAsync(int groupId, int numberItemsToSkip);
        Task<ApiResponse<PublicGroupMessageDto>> GetByMessageIdAsync(Guid messageId);
        Task<ApiResponse<List<PublicGroupMessageDto>>> GetListByUserIdAsync(string userId, int numberItemsToSkip);
        Task<ApiResponse<PublicGroupMessageDto>> ModifyAsync(ModifyPublicGroupMessageDto dtoToModify, string jwtUserId);
        Task<bool> DeleteAllMessagesInGroupAsync(int chatGroupId);
    }
}