using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IChatGroupsDataAccess
    {
        Task<bool> AddAsync(PublicChatGroups chatGroup);
        bool GroupExists(int groupId);
        Task<bool> DeleteAsync(PublicChatGroups chatGroup);
        Task<ChatGroupsView> GetChatGroupByIdAsync(int id);
        Task<List<ChatGroupsView>> GetViewListPrivateByUserIdAsync(Guid userId);
        Task<List<ChatGroupsView>> GetViewListPublicChatGroupsAsync();
        Task<bool> ModifyAsync(PublicChatGroups chatGroup);
        Task<bool> AddUserToPrivateChatGroupAsync(PrivateGroupMembers privateGroupMember);
        Task<PrivateGroupMembers> GetPrivateGroupMemberRecord(int chatGroupid, string userId);
        Task<bool> RemoveUserFromPrivateChatGroup(PrivateGroupMembers privateGroupMember);
        Task<bool> IsPublicChatGroup(int groupId);
        PublicChatGroups GetByGroupName(string chatGroupName);
        bool GroupNameTaken(string chatGroupName);
    }
}