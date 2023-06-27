using ChatApplicationModels;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IChatGroupsDataAccess
    {
        Task AddChatGroupAsync(ChatGroups chatGroup);
        Task<bool> ChatGroupexists(int groupId);
        Task DeleteChatGroupAsync(ChatGroups chatGroup);
        Task<ChatGroups> GetChatGroupByIdAsync(int id);
        List<ChatGroups> GetPrivateChatGroupsByUserId(string userId);
        Task<List<ChatGroups>> GetPublicChatGroupsAsync();
        Task ModifyChatGroup(ChatGroups chatGroup);
        Task AddUserToPrivateChatGroup(int chatGroupid, string userId);
        Task RemoveUserFromPrivateChatGroup(int chatGroupid, string userId);
    }
}