using ChatApplicationModels;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPublicMessagesDataAccess
    {
        Task<bool> AddMessageAsync(PublicMessages message);
        Task<bool> DeleteMessage(PublicMessages message);
        Task<List<PublicMessages>> GetMessagesByGroupIdAsync(int groupId, int currentItemCount);
        Task<List<PublicMessages>> GetMessagesByUserIdAsync(string userId, int currentItemCount);
        Task<PublicMessages> GetPublicMessageByIdAsync(string messageId);
        Task<bool> ModifyMessageAsync(PublicMessages message);
        Task<bool> PublicMessageExists(string messageId);
    }
}