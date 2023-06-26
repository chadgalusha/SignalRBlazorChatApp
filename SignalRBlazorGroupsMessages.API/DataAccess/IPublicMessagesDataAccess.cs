using ChatApplicationModels;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPublicMessagesDataAccess
    {
        Task AddMessageAsync(PublicMessages message);
        Task DeleteMessage(PublicMessages message);
        Task<List<PublicMessages>> GetMessagesByGroupIdAsync(int groupId);
        Task<List<PublicMessages>> GetMessagesByUserIdAsync(string userId);
        Task<PublicMessages> GetPublicMessageByIdAsync(string messageId);
        Task ModifyMessageAsync(PublicMessages message);
        Task<bool> PublicMessageExists(string messageId);
    }
}