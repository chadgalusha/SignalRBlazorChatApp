using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPublicMessagesDataAccess
    {
        Task<bool> AddAsync(PublicGroupMessages message);
        Task<bool> DeleteAsync(PublicGroupMessages message);
        Task<List<PublicGroupMessageDto>> GetDtoListByGroupIdAsync(int groupId, int numberItemsToSkip);
        Task<List<PublicGroupMessageDto>> GetDtoListByUserIdAsync(string userId, int numberItemsToSkip);
        Task<PublicGroupMessageDto> GetDtoByMessageIdAsync(Guid messageId);
        Task<bool> ModifyAsync(PublicGroupMessages message);
        Task<bool> Exists(Guid messageId);
        Task<PublicGroupMessages> GetByMessageIdAsync(Guid messageId);
        Task<bool> DeleteMessagesByResponseMessageIdAsync(Guid responseMessageId);
        Task<bool> DeleteAllMessagesInGroupAsync(int chatGroupId);
    }
}