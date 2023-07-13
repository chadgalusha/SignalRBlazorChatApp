using ChatApplicationModels;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPrivateMessagesDataAccess
    {
        Task<bool> AddPrivateMessageAsync(PrivateUserMessages privateMessage);
        Task<bool> DeletePrivateMessageAsync(PrivateUserMessages privateMessage);
        Task<List<PrivateUserMessages>> GetAllPrivateMessagesForUserAsync(string userId);
        PrivateUserMessages GetPrivateMessage(int privateMessageId);
        Task<List<PrivateUserMessages>> GetPrivateMessagesFromUserAsync(string toUserId, string fromUserId);
        Task<bool> ModifyPrivateMessageAsync(PrivateUserMessages privateMessage);
    }
}