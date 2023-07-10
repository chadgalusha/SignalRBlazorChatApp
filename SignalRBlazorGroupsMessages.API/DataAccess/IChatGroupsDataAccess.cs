using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IChatGroupsDataAccess
    {
        Task<bool> AddChatGroupAsync(ChatGroups chatGroup);
        bool ChatGroupexists(int groupId);
        Task<bool> DeleteChatGroupAsync(ChatGroups chatGroup);
        Task<ChatGroupsView> GetChatGroupByIdAsync(int id);
        Task<List<ChatGroupsView>> GetViewListPrivateChatGroupsByUserId(Guid userId);
        Task<List<ChatGroupsView>> GetViewListPublicChatGroupsAsync();
        Task<bool> ModifyChatGroupAsync(ChatGroups chatGroup);
        Task<bool> AddUserToPrivateChatGroupAsync(PrivateGroupMembers privateGroupMember);
        Task<PrivateGroupMembers> GetPrivateGroupMemberRecord(int chatGroupid, string userId);
        Task<bool> RemoveUserFromPrivateChatGroup(PrivateGroupMembers privateGroupMember);
        Task<bool> IsPublicChatGroup(int groupId);
    }
}