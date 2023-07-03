using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPublicMessagesDataAccess
    {
        Task<bool> AddMessageAsync(PublicMessages message);
        Task<bool> DeleteMessageAsync(PublicMessages message);
        Task<List<PublicMessagesView>> GetMessagesByGroupIdAsync(int groupId, int numberItemsToSkip);
        Task<List<PublicMessagesView>> GetMessagesByUserIdAsync(Guid userId, int numberItemsToSkip);
        Task<PublicMessagesView> GetPublicMessageByIdAsync(Guid messageId);
        Task<bool> ModifyMessageAsync(PublicMessages message);
        Task<bool> PublicMessageExists(Guid messageId);
    }
}