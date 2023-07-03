using ChatApplicationModels;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IChatGroupsDataAccess
    {
        Task<bool> AddChatGroupAsync(ChatGroups chatGroup);
        bool ChatGroupexists(int groupId);
        Task<bool> DeleteChatGroupAsync(ChatGroups chatGroup);
        Task<ChatGroups> GetChatGroupByIdAsync(int id);
        List<ChatGroups> GetPrivateChatGroupsByUserId(string userId);
        Task<List<ChatGroups>> GetPublicChatGroupsAsync();
        Task<bool> ModifyChatGroupAsync(ChatGroups chatGroup);
        Task<bool> AddUserToPrivateChatGroupAsync(PrivateGroupMembers privateGroupMember);
        Task<PrivateGroupMembers> GetPrivateGroupMemberRecord(int chatGroupid, string userId);
        Task<bool> RemoveUserFromPrivateChatGroup(PrivateGroupMembers privateGroupMember);
    }
}