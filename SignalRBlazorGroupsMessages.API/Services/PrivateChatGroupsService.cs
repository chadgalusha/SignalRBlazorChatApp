using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;

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

        public async Task<ApiResponse<List<PrivateChatGroupsDto>>> GetViewListPrivateByUserId(Guid userId)
        {
            ApiResponse<List<PrivateChatGroupsDto>> response = new();

            if (userId == new Guid())
            {
                response = ReturnApiResponse.Failure(response, "invalid userId");
                response.Data = null;
                return response;
            }

            try
            {
                List<PrivateChatGroupsView> viewList = await _privateGroupsDataAccess.GetViewListPrivateByUserIdAsync(userId);
                List<PrivateChatGroupsDto> dtoList = ViewListToDtoList(viewList);

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

        private List<PrivateChatGroupsDto> ViewListToDtoList(List<PrivateChatGroupsView> viewList)
        {
            List<PrivateChatGroupsDto> dtoList = new();

            foreach (PrivateChatGroupsView view in viewList)
            {
                PrivateChatGroupsDto dto = new()
                {
                    ChatGroupId = view.ChatGroupId,
                    ChatGroupName = view.ChatGroupName,
                    GroupCreated = view.GroupCreated,
                    GroupOwnerUserId = view.GroupOwnerUserId,
                    UserName = view.UserName
                };
                dtoList.Add(dto);
            }
            return dtoList;
        }

        private PrivateChatGroupsDto ViewToDto(PrivateChatGroupsView view)
        {
            return new()
            {
                ChatGroupId = view.ChatGroupId,
                ChatGroupName = view.ChatGroupName,
                GroupCreated = view.GroupCreated,
                GroupOwnerUserId = view.GroupOwnerUserId,
                UserName = view.UserName
            };
        }

        private PrivateChatGroups DtoToNewChatGroup(PrivateChatGroupsDto dto)
        {
            return new()
            {
                ChatGroupName = dto.ChatGroupName,
                GroupCreated = dto.GroupCreated,
                GroupOwnerUserId = dto.GroupOwnerUserId
            };
        }

        private PrivateChatGroupsDto NewChatGroupToDto(PrivateChatGroups newGroup, string userName)
        {
            return new()
            {
                ChatGroupId = newGroup.ChatGroupId,
                ChatGroupName = newGroup.ChatGroupName,
                GroupCreated = newGroup.GroupCreated,
                GroupOwnerUserId = newGroup.GroupOwnerUserId,
                UserName = userName
            };
        }

        private PrivateChatGroups DtoToChatGroup(PrivateChatGroupsDto dto)
        {
            return new()
            {
                ChatGroupId = dto.ChatGroupId,
                ChatGroupName = dto.ChatGroupName,
                GroupCreated = dto.GroupCreated,
                GroupOwnerUserId = dto.GroupOwnerUserId
            };
        }

        #endregion
    }
}
