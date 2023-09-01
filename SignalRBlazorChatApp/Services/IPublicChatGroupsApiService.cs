using ChatApplicationModels.Dtos;
using SignalRBlazorChatApp.Models;

namespace SignalRBlazorChatApp.Services
{
    public interface IPublicChatGroupsApiService
    {
        Task<ApiResponse<PublicChatGroupsDto>> DeleteGroup(int groupId, string jsonWebToken);
        Task<ApiResponse<List<PublicChatGroupsDto>>> GetPublicChatGroupsAsync(string jsonWebToken);
        Task<ApiResponse<PublicChatGroupsDto>> PostNewGroup(CreatePublicChatGroupDto createDto, string jsonWebToken);
        Task<ApiResponse<PublicChatGroupsDto>> UpdateGroup(ModifyPublicChatGroupDto modifyDto, string jsonWebToken);
    }
}