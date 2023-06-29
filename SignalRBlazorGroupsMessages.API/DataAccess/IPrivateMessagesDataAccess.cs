using ChatApplicationModels;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPrivateMessagesDataAccess
    {
        Task AddPrivateMessageAsync(PrivateMessages privateMessage);
        Task DeletePrivateMessageAsync(PrivateMessages privateMessage);
        Task<List<PrivateMessages>> GetAllPrivateMessagesForUserAsync(string userId);
        PrivateMessages GetPrivateMessage(int privateMessageId);
        Task<List<PrivateMessages>> GetPrivateMessagesFromUserAsync(string toUserId, string fromUserId);
        Task ModifyPrivateMessageAsync(PrivateMessages privateMessage);
    }
}