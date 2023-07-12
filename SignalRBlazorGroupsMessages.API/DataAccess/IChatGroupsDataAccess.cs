using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IChatGroupsDataAccess
    {
        Task<bool> AddAsync(ChatGroups chatGroup);
        bool GroupExists(int groupId);
        Task<bool> DeleteAsync(ChatGroups chatGroup);
        Task<ChatGroupsView> GetChatGroupByIdAsync(int id);
        Task<List<ChatGroupsView>> GetViewListPrivateByUserIdAsync(Guid userId);
        Task<List<ChatGroupsView>> GetViewListPublicChatGroupsAsync();
        Task<bool> ModifyAsync(ChatGroups chatGroup);
        Task<bool> AddUserToPrivateChatGroupAsync(PrivateGroupMembers privateGroupMember);
        Task<PrivateGroupMembers> GetPrivateGroupMemberRecord(int chatGroupid, string userId);
        Task<bool> RemoveUserFromPrivateChatGroup(PrivateGroupMembers privateGroupMember);
        Task<bool> IsPublicChatGroup(int groupId);
        ChatGroups GetByGroupName(string chatGroupName);
        bool GroupNameTaken(string chatGroupName);
    }
}