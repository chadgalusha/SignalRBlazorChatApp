using ChatApplicationModels;
using ChatApplicationModels.Dtos;
using SignalRBlazorChatApp.Models;

namespace SignalRBlazorChatApp.Services
{
    public interface IPrivateChatGroupsApiService
    {
        Task<ApiResponse<PrivateChatGroupsDto>> DeleteGroup(int groupId, string jsonWebToken);
        Task<ApiResponse<List<PrivateChatGroupsDto>>> GetPrivateChatGroupsAsync(string userId, string jsonWebToken);
        Task<ApiResponse<PrivateChatGroupsDto>> PostNewGroup(CreatePrivateChatGroupDto createDto, string jsonWebToken);
        Task<ApiResponse<PrivateGroupMembers>> PostGroupMember(int groupId, string userId, string jsonWebToken);
        Task<ApiResponse<PrivateChatGroupsDto>> UpdateGroup(ModifyPrivateChatGroupDto modifyDto, string jsonWebToken);
    }
}