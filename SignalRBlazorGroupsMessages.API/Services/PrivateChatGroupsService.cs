using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public class PrivateChatGroupsService
    {
        private readonly IPrivateChatGroupsDataAccess _privateGroupsDataAccess;
        private readonly ISerilogger _serilogger;

        public PrivateChatGroupsService(IPrivateChatGroupsDataAccess privateGroupsDataAccess, ISerilogger serilogger)
        {
            _privateGroupsDataAccess = privateGroupsDataAccess ?? throw new Exception(nameof(privateGroupsDataAccess));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        public async Task<ApiResponse<List<PrivateChatGroupsDto>>> GetViewListPrivateByUserId(string userId)
        {
            ApiResponse<List<PrivateChatGroupsDto>> response = new();

            try
            {
                List<PrivateChatGroupsDto> dtoList = await _privateGroupsDataAccess.GetDtoListByUserIdAsync(userId);

                return ReturnApiResponse.Success(response, dtoList);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.GetViewListPrivateByUserId", ex);

                response = ReturnApiResponse.Failure(response, "Error getting private chat groups.");
                response.Data = null;

                return response;
            }
        }

        #region PRIVATE METHODS

        private PrivateChatGroups DtoToNewChatGroup(PrivateChatGroupsDto dto)
        {
            return new()
            {
                ChatGroupName    = dto.ChatGroupName,
                GroupCreated     = dto.GroupCreated,
                GroupOwnerUserId = dto.GroupOwnerUserId.ToString()
            };
        }

        private PrivateChatGroupsDto ModelGroupToDto(PrivateChatGroups newGroup, string userName)
        {
            return new()
            {
                ChatGroupId      = newGroup.ChatGroupId,
                ChatGroupName    = newGroup.ChatGroupName,
                GroupCreated     = newGroup.GroupCreated,
                GroupOwnerUserId = newGroup.GroupOwnerUserId,
                UserName         = userName
            };
        }

        private PrivateChatGroups DtoToModel(PrivateChatGroupsDto dto)
        {
            return new()
            {
                ChatGroupId      = dto.ChatGroupId,
                ChatGroupName    = dto.ChatGroupName,
                GroupCreated     = dto.GroupCreated,
                GroupOwnerUserId = dto.GroupOwnerUserId.ToString()
            };
        }

        #endregion
    }
}
