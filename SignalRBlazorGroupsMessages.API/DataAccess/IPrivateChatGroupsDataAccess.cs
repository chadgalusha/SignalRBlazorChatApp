using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models.Views;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPrivateChatGroupsDataAccess
    {
        Task<bool> AddUserToPrivateChatGroupAsync(PrivateGroupMembers privateGroupMember);
        Task<PrivateGroupMembers> GetPrivateGroupMemberRecord(int chatGroupid, string userId);
        Task<List<PrivateChatGroupsView>> GetViewListPrivateByUserIdAsync(Guid userId);
        Task<bool> RemoveUserFromPrivateChatGroup(PrivateGroupMembers privateGroupMember);
    }
}