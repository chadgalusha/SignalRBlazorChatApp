using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public interface IPublicChatGroupsService
    {
        Task<ApiResponse<PublicChatGroupsDto>> AddAsync(PublicChatGroupsDto dto);
        Task<ApiResponse<PublicChatGroupsDto>> DeleteAsync(PublicChatGroupsDto dto);
        Task<ApiResponse<PublicChatGroupsDto>> GetByIdAsync(int groupId);
        Task<ApiResponse<List<PublicChatGroupsDto>>> GetListPublicChatGroupsAsync();
        Task<ApiResponse<PublicChatGroupsDto>> ModifyAsync(PublicChatGroupsDto dto);
    }
}