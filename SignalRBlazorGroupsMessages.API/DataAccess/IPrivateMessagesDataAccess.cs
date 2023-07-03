using ChatApplicationModels;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPrivateMessagesDataAccess
    {
        Task<bool> AddPrivateMessageAsync(PrivateMessages privateMessage);
        Task<bool> DeletePrivateMessageAsync(PrivateMessages privateMessage);
        Task<List<PrivateMessages>> GetAllPrivateMessagesForUserAsync(string userId);
        PrivateMessages GetPrivateMessage(int privateMessageId);
        Task<List<PrivateMessages>> GetPrivateMessagesFromUserAsync(string toUserId, string fromUserId);
        Task<bool> ModifyPrivateMessageAsync(PrivateMessages privateMessage);
    }
}