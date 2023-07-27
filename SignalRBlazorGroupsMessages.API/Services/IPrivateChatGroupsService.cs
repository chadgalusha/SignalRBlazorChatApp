using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public interface IPrivateChatGroupsService
    {
        Task<ApiResponse<PrivateChatGroupsDto>> AddAsync(CreatePrivateChatGroupDto createDto);
        Task<ApiResponse<PrivateGroupMembers>> AddPrivateGroupMember(int groupId, string userId);
        Task<ApiResponse<PrivateChatGroupsDto>> DeleteAsync(int groupId, string jwtUserId);
        Task<ApiResponse<PrivateChatGroupsDto>> GetDtoByGroupIdAsync(int groupId, string userId);
        Task<ApiResponse<List<PrivateChatGroupsDto>>> GetDtoListByUserIdAsync(string userId);
        Task<ApiResponse<PrivateChatGroupsDto>> ModifyAsync(ModifyPrivateChatGroupDto modifyDto, string jwtUserId);
        Task<ApiResponse<PrivateGroupMembers>> RemoveUserFromGroupAsync(int groupId, string userId);
    }
}