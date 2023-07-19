using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public interface IPublicChatGroupsService
    {
        Task<ApiResponse<PublicChatGroupsDto>> AddAsync(CreatePublicChatGroupDto dto);
        Task<ApiResponse<PublicChatGroupsDto>> DeleteAsync(int dtoGroupId);
        Task<ApiResponse<PublicChatGroupsDto>> GetDtoByIdAsync(int groupId);
        Task<ApiResponse<List<PublicChatGroupsDto>>> GetListPublicChatGroupsAsync();
        Task<ApiResponse<PublicChatGroupsDto>> ModifyAsync(ModifyPublicChatGroupDto dto);
    }
}