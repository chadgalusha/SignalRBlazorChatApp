using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public class PublicChatGroupsService
    {
        private readonly IPublicChatGroupsDataAccess _publicGroupsDataAccess;
        private readonly IPublicMessagesDataAccess _publicMessagesDataAccess;
        private readonly ISerilogger _serilogger;

        public PublicChatGroupsService(IPublicChatGroupsDataAccess publicGroupsDataAccess, IPublicMessagesDataAccess publicMessagesDataAccess, ISerilogger serilogger) 
        {
            _publicGroupsDataAccess = publicGroupsDataAccess ?? throw new Exception(nameof(publicGroupsDataAccess));
            _publicMessagesDataAccess = publicMessagesDataAccess ?? throw new Exception(nameof(publicMessagesDataAccess));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        public async Task<ApiResponse<List<PublicChatGroupsDto>>> GetViewListPublicChatGroupsAsync()
        {
            ApiResponse<List<PublicChatGroupsDto>> response = new();

            try
            {
                List<PublicChatGroupsView> viewList = await _publicGroupsDataAccess.GetViewListPublicChatGroupsAsync();
                List<PublicChatGroupsDto> dtoList = ViewListToDtoList(viewList);

                return ReturnApiResponse.Success(response, dtoList);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.ChatGroupsService", ex);

                response = ReturnApiResponse.Failure(response, "Error getting public chat groups.");
                response.Data = null;

                return response;
            }
        }

        public async Task<ApiResponse<List<PublicChatGroupsDto>>> GetViewListPrivateByUserId(Guid userId)
        {
            ApiResponse<List<PublicChatGroupsDto>> response = new();

            if (userId == new Guid())
            {
                response = ReturnApiResponse.Failure(response, "invalid userId");
                response.Data = null;
                return response;
            }

            try
            {
                List<PublicChatGroupsView> viewList = await _publicGroupsDataAccess.GetViewListPrivateByUserIdAsync(userId);
                List<PublicChatGroupsDto> dtoList = ViewListToDtoList(viewList);

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

        public async Task<ApiResponse<PublicChatGroupsDto>> GetChatGroupByIdAsync(int groupId)
        {
            ApiResponse<PublicChatGroupsDto> response = new();

            try
            {
                if (!GroupExists(groupId))
                {
                    response = ReturnApiResponse.Failure(response, "invalid groupId");
                    response.Data = null;
                    return response;
                }

                PublicChatGroupsView view = await _publicGroupsDataAccess.GetChatGroupByIdAsync(groupId);
                PublicChatGroupsDto dtoGroup = ViewToDto(view);

                return ReturnApiResponse.Success(response, dtoGroup);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.GetChatGroupByIdAsync", ex);

                response = ReturnApiResponse.Failure(response, "Error getting chat group.");
                response.Data = null;

                return response;
            }
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> AddAsync(PublicChatGroupsDto dto)
        {
            ApiResponse<PublicChatGroupsDto> response = new();

            try
            {
                if (GroupNameTaken(dto.ChatGroupName) == true)
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, "Chat Group name alrady taken.");
                }

                PublicChatGroups newChatGroup = DtoToNewChatGroup(dto);
                bool isSuccess = await _publicGroupsDataAccess.AddAsync(newChatGroup);

                if (!isSuccess)
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, "Error saving new chat group.");
                }

                PublicChatGroupsDto newDto = NewChatGroupToDto(newChatGroup, dto.ChatGroupName);
                return ReturnApiResponse.Success(response, newDto);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.AddAsync", ex);

                response = ReturnApiResponse.Failure(response, "Error creating new chat group.");
                response.Data = null;

                return response;
            }
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> ModifyAsync(PublicChatGroupsDto dto)
        {
            ApiResponse<PublicChatGroupsDto> response = new();

            try
            {
                PublicChatGroupsView existingView = await _publicGroupsDataAccess.GetChatGroupByIdAsync(dto.ChatGroupId);

                // check if chat group passes modify checks 1st. if false return failure with error messages.
                (bool, string) messageChecks = ModifyChatGroupChecks(dto, existingView);
                if (messageChecks.Item1 == false)
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, messageChecks.Item2);
                }

                PublicChatGroups chatGroupToModify = DtoToChatGroup(dto);
                bool isSuccess = await _publicGroupsDataAccess.ModifyAsync(chatGroupToModify);

                if (!isSuccess)
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, "Error modifying chat group");
                }

                return ReturnApiResponse.Success(response, dto);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.ModifyAsync", ex);

                response = ReturnApiResponse.Failure(response, "Error modifying chat group.");
                response.Data = null;

                return response;
            }
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> DeleteAsync(PublicChatGroupsDto dto)
        {
            ApiResponse<PublicChatGroupsDto> response = new();

            try
            {
                if (!_publicGroupsDataAccess.GroupExists(dto.ChatGroupId))
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, "Chat Group Id not found");
                }

                

                return ReturnApiResponse.Success(response, dto);//FIX THIS
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.DeleteAsync", ex);

                response = ReturnApiResponse.Failure(response, "Error deleting chat group.");
                response.Data = null;

                return response;
            }
        }

        #region PRIVATE METHODS

        private bool GroupExists(int groupId)
        {
            return _publicGroupsDataAccess.GroupExists(groupId);
        }

        private bool GroupNameTaken(string chatGroupName)
        {
            return _publicGroupsDataAccess.GroupNameTaken(chatGroupName);
        }

        private (bool, string) ModifyChatGroupChecks(PublicChatGroupsDto dto, PublicChatGroupsView view)
        {
            bool passesChecks = true;
            string errorMessage = "";

            if (dto.ChatGroupName != view.ChatGroupName)
            {
                if (GroupNameTaken(dto.ChatGroupName) == true)
                {
                    passesChecks = false;
                    errorMessage += "[Chat Group name alrady taken.]";
                }
            }
            if (dto.GroupOwnerUserId != view.GroupOwnerUserId)
            {
                passesChecks = false;
                errorMessage += "[Must use change group owner request for different group owner.]";
            }
            if (dto.PrivateGroup != view.PrivateGroup)
            {
                passesChecks = false;
                errorMessage += "[Cannot change status of chat group from private to public, and vice versa.]";
            }

            return (passesChecks, errorMessage);
        }

        private List<PublicChatGroupsDto> ViewListToDtoList(List<PublicChatGroupsView> viewList)
        {
            List<PublicChatGroupsDto> dtoList = new();

            foreach (PublicChatGroupsView view in viewList)
            {
                PublicChatGroupsDto dto = new()
                {
                    ChatGroupId      = view.ChatGroupId,
                    ChatGroupName    = view.ChatGroupName,
                    GroupCreated     = view.GroupCreated,
                    GroupOwnerUserId = view.GroupOwnerUserId,
                    UserName         = view.UserName,
                    PrivateGroup     = view.PrivateGroup
                };
                dtoList.Add(dto);
            }
            return dtoList;
        }

        private PublicChatGroupsDto ViewToDto(PublicChatGroupsView view)
        {
            return new()
            {
                ChatGroupId      = view.ChatGroupId,
                ChatGroupName    = view.ChatGroupName,
                GroupCreated     = view.GroupCreated,
                GroupOwnerUserId = view.GroupOwnerUserId,
                UserName         = view.UserName,
                PrivateGroup     = view.PrivateGroup
            };
        }

        private PublicChatGroups DtoToNewChatGroup(PublicChatGroupsDto dto)
        {
            return new()
            {
                ChatGroupName    = dto.ChatGroupName,
                GroupCreated     = dto.GroupCreated,
                GroupOwnerUserId = dto.GroupOwnerUserId,
                PrivateGroup     = dto.PrivateGroup
            };
        }

        private PublicChatGroupsDto NewChatGroupToDto(PublicChatGroups newGroup, string userName)
        {
            return new()
            {
                ChatGroupId      = newGroup.ChatGroupId,
                ChatGroupName    = newGroup.ChatGroupName,
                GroupCreated     = newGroup.GroupCreated,
                GroupOwnerUserId = newGroup.GroupOwnerUserId,
                UserName         = userName,
                PrivateGroup     = newGroup.PrivateGroup
            };
        }

        private PublicChatGroups DtoToChatGroup(PublicChatGroupsDto dto) 
        {
            return new()
            {
                ChatGroupId      = dto.ChatGroupId,
                ChatGroupName    = dto.ChatGroupName,
                GroupCreated     = dto.GroupCreated,
                GroupOwnerUserId = dto.GroupOwnerUserId,
                PrivateGroup     = dto.PrivateGroup
            };
        }

        #endregion
    }
}
