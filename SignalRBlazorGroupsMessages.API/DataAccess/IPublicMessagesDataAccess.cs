using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPublicMessagesDataAccess
    {
        Task<bool> AddAsync(PublicGroupMessages message);
        Task<bool> DeleteAsync(PublicGroupMessages message);
        Task<List<PublicMessagesView>> GetViewListByGroupIdAsync(int groupId, int numberItemsToSkip);
        Task<List<PublicMessagesView>> GetViewListByUserIdAsync(Guid userId, int numberItemsToSkip);
        Task<PublicMessagesView> GetViewByMessageIdAsync(Guid messageId);
        Task<bool> ModifyAsync(PublicGroupMessages message);
        Task<bool> Exists(Guid messageId);
        Task<PublicGroupMessages> GetByMessageIdAsync(Guid messageId);
        Task<bool> DeleteMessagesByResponseMessageIdAsync(Guid responseMessageId);
        Task<bool> DeleteMessagesFromChatGroupAsync(int chatGroupId);
    }
}