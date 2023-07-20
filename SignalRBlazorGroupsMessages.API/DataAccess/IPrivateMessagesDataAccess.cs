using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPrivateMessagesDataAccess
    {
        Task<bool> AddAsync(PrivateUserMessages newMessage);
        Task<bool> DeleteAllMessagesInGroupAsync(int chatgroupId);
        Task<bool> DeleteAsync(PrivateUserMessages deleteMessage);
        Task<bool> DeleteMessagesByResponseMessageIdAsync(Guid responseMessageId);
        Task<bool> Exists(Guid messageId);
        Task<PrivateGroupMessages> GetByMessageIdAsync(Guid messageId);
        Task<PrivateGroupMessageDto> GetDtoByMessageIdAsync(Guid messageId);
        Task<List<PrivateGroupMessageDto>> GetDtoListByGroupIdAsync(int groupId, int numberMessagesToSkip);
        Task<List<PrivateGroupMessageDto>> GetDtoListByUserIdAsync(string userId, int numberMessagesToSkip);
        Task<bool> ModifyAsync(PrivateUserMessages modifyMessage);
    }
}