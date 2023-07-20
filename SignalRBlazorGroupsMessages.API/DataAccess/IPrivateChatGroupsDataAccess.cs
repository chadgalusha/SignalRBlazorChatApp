using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPrivateChatGroupsDataAccess
    {
        Task<bool> AddUserToPrivateChatGroupAsync(PrivateGroupMembers privateGroupMember);
        Task<PrivateGroupMembers> GetPrivateGroupMemberRecord(int chatGroupid, string userId);
        Task<List<PrivateChatGroupsDto>> GetDtoListByUserIdAsync(string userId);
        Task<bool> RemoveUserFromPrivateChatGroup(PrivateGroupMembers privateGroupMember);
        PrivateChatGroups GetByGroupname(string groupName);
        PrivateChatGroups GetByGroupId(int groupId);
        bool GroupNameTaken(string groupName);
        Task<bool> IsUserInPrivateGroup(int groupId, string userId);
        Task<bool> AddAsync(PrivateChatGroups newGroup);
        Task<bool> ModifyAsync(PrivateChatGroups modifiedGroup);
        Task<bool> DeleteAsync(PrivateChatGroups deleteGroup);
    }
}