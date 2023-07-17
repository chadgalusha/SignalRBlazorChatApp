using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models.Views;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPublicChatGroupsDataAccess
    {
        Task<bool> AddAsync(PublicChatGroups chatGroup);
        bool GroupExists(int groupId);
        Task<bool> DeleteAsync(PublicChatGroups chatGroup);
        Task<PublicChatGroupsView> GetViewByIdAsync(int id);
        Task<List<PublicChatGroupsView>> GetViewListAsync();
        Task<bool> ModifyAsync(PublicChatGroups chatGroup);
        PublicChatGroups GetByGroupName(string chatGroupName);
        bool GroupNameTaken(string chatGroupName);
        PublicChatGroups GetByGroupId(int groupId);
    }
}