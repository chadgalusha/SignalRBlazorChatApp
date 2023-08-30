using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPrivateGroupMessagesDataAccess
    {
        Task<bool> AddAsync(PrivateGroupMessages newMessage);
        Task<bool> DeleteAllMessagesInGroupAsync(int chatgroupId);
        Task<bool> DeleteAsync(PrivateGroupMessages deleteMessage);
        Task<bool> DeleteMessagesByReplyMessageIdAsync(Guid responseMessageId);
        Task<bool> MessageIdExists(Guid messageId);
        Task<PrivateGroupMessages> GetByMessageIdAsync(Guid messageId);
        Task<PrivateGroupMessageDto> GetDtoByMessageIdAsync(Guid messageId);
        Task<List<PrivateGroupMessageDto>> GetDtoListByGroupIdAsync(int groupId, int numberMessagesToSkip);
        Task<List<PrivateGroupMessageDto>> GetDtoListByUserIdAsync(string userId, int numberMessagesToSkip);
        Task<bool> ModifyAsync(PrivateGroupMessages modifyMessage);
        Task<bool> GroupIdExists(int groupId);
    }
}