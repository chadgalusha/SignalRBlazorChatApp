using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public interface IPublicChatGroupsService
    {
        Task<ApiResponse<PublicChatGroupsDto>> AddAsync(CreatePublicChatGroupDto dto);
        Task<ApiResponse<PublicChatGroupsDto>> DeleteAsync(int dtoGroupId, string jwtUserId);
        Task<ApiResponse<PublicChatGroupsDto>> GetByIdAsync(int groupId);
        Task<ApiResponse<List<PublicChatGroupsDto>>> GetListAsync();
        Task<ApiResponse<PublicChatGroupsDto>> ModifyAsync(ModifyPublicChatGroupDto dto, string jwtUserId);
    }
}