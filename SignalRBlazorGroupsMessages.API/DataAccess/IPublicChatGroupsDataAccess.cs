using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPublicChatGroupsDataAccess
    {
        Task<bool> AddAsync(PublicChatGroups chatGroup);
        bool GroupExists(int groupId);
        Task<bool> DeleteAsync(PublicChatGroups chatGroup);
        Task<PublicChatGroupsDto> GetDtoByIdAsync(int id);
        Task<List<PublicChatGroupsDto>> GetDtoListAsync();
        Task<bool> ModifyAsync(PublicChatGroups chatGroup);
        PublicChatGroups GetByGroupName(string chatGroupName);
        bool GroupNameTaken(string chatGroupName);
        PublicChatGroups GetByGroupId(int groupId);
    }
}