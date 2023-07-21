using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public interface IPrivateGroupMessagesService
    {
        Task<ApiResponse<PrivateGroupMessageDto>> AddAsync(CreatePrivateGroupMessageDto createDto);
        Task<bool> DeleteAllMessagesInGroupAsync(int groupId);
        Task<ApiResponse<PrivateGroupMessageDto>> DeleteAsync(Guid messageId);
        Task<ApiResponse<PrivateGroupMessageDto>> GetDtoByMessageIdAsync(Guid messageId);
        Task<ApiResponse<List<PrivateGroupMessageDto>>> GetDtoListByGroupIdAsync(int groupId, int numberMessagesToSkip);
        Task<ApiResponse<List<PrivateGroupMessageDto>>> GetDtoListByUserIdAsync(string userId, int numberMessagesToSkip);
        Task<ApiResponse<PrivateGroupMessageDto>> ModifyAsync(ModifyPrivateGroupMessageDto modifyDto);
    }
}