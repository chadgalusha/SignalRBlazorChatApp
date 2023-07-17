using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Models.Views;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public class PublicChatGroupsService : IPublicChatGroupsService
    {
        private readonly IPublicChatGroupsDataAccess _publicGroupsDataAccess;
        private readonly IPublicMessagesService _publicMessagesService;
        private readonly ISerilogger _serilogger;

        public PublicChatGroupsService(IPublicChatGroupsDataAccess publicGroupsDataAccess, IPublicMessagesService publicMessagesService, ISerilogger serilogger)
        {
            _publicGroupsDataAccess = publicGroupsDataAccess ?? throw new Exception(nameof(publicGroupsDataAccess));
            _publicMessagesService = publicMessagesService ?? throw new Exception(nameof(publicMessagesService));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        public async Task<ApiResponse<List<PublicChatGroupsDto>>> GetListPublicChatGroupsAsync()
        {
            ApiResponse<List<PublicChatGroupsDto>> response = new();

            try
            {
                List<PublicChatGroupsView> viewList = await _publicGroupsDataAccess.GetViewListAsync();
                List<PublicChatGroupsDto> dtoList = ViewListToDtoList(viewList);

                return ReturnApiResponse.Success(response, dtoList);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.ChatGroupsService", ex);

                response.Data = null;
                return ReturnApiResponse.Failure(response, "Error getting public chat groups.");
            }
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> GetViewByIdAsync(int groupId)
        {
            ApiResponse<PublicChatGroupsDto> response = new();

            try
            {
                if (!GroupExists(groupId))
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, "invalid groupId");
                }

                PublicChatGroupsView view = await _publicGroupsDataAccess.GetViewByIdAsync(groupId);
                PublicChatGroupsDto dtoGroup = ViewToDto(view);

                return ReturnApiResponse.Success(response, dtoGroup);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.GetChatGroupByIdAsync", ex);

                response.Data = null;
                return ReturnApiResponse.Failure(response, "Error getting chat group.");
            }
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> AddAsync(CreatePublicChatGroupDto dto)
        {
            ApiResponse<PublicChatGroupsDto> response = new();

            try
            {
                if (GroupNameTaken(dto.ChatGroupName) == true)
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, "Chat Group name already taken.");
                }

                PublicChatGroups newChatGroup = CreateDtoToNewModel(dto);
                bool isSuccess = await _publicGroupsDataAccess.AddAsync(newChatGroup);

                if (!isSuccess)
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, "Error saving new chat group.");
                }

                PublicChatGroupsDto newDto = ModelToDto(newChatGroup, dto.ChatGroupName);
                return ReturnApiResponse.Success(response, newDto);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.AddAsync", ex);

                response.Data = null;
                return ReturnApiResponse.Failure(response, "Error creating new chat group.");
            }
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> ModifyAsync(ModifyPublicChatGroupDto dtoToModify)
        {
            ApiResponse<PublicChatGroupsDto> response = new();

            try
            {
                PublicChatGroupsView existingView = await _publicGroupsDataAccess.GetViewByIdAsync(dtoToModify.ChatGroupId);

                // check if chat group passes modify checks 1st. if false return failure with error messages.
                (bool, string) messageChecks = ModifyChatGroupChecks(dtoToModify, existingView);
                if (messageChecks.Item1 == false)
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, messageChecks.Item2);
                }

                PublicChatGroups chatGroupToModify = ModifyDtoToModel(dtoToModify, existingView);
                bool isSuccess = await _publicGroupsDataAccess.ModifyAsync(chatGroupToModify);

                if (!isSuccess)
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, "Error modifying chat group");
                }

                return ReturnApiResponse.Success(response, ModelToDto(chatGroupToModify, existingView.UserName));
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.ModifyAsync", ex);

                response.Data = null;
                return ReturnApiResponse.Failure(response, "Error modifying chat group.");
            }
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> DeleteAsync(int groupId)
        {
            ApiResponse<PublicChatGroupsDto> response = new();

            try
            {
                if (!_publicGroupsDataAccess.GroupExists(groupId))
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, "Chat Group Id not found");
                }

                // delete all messages in a public group
                bool deleteMessagesResult = await _publicMessagesService.DeleteAllMessagesInGroupAsync(groupId);
                if (!deleteMessagesResult)
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, "Error deleting messages from this group");
                }

                // get group model to delete, assign to dto for return data
                PublicChatGroups groupToDelete = _publicGroupsDataAccess.GetByGroupId(groupId);
                PublicChatGroupsDto dtoToReturn = ModelToDto(groupToDelete, "");

                // delete the chat group
                bool deleteChatGroupResponse = await _publicGroupsDataAccess.DeleteAsync(groupToDelete);
                if (!deleteChatGroupResponse)
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, "Error delete the chat group.");
                }

                return ReturnApiResponse.Success(response, dtoToReturn);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.DeleteAsync", ex);

                response.Data = null;
                return ReturnApiResponse.Failure(response, "Error deleting chat group.");
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

        private (bool, string) ModifyChatGroupChecks(ModifyPublicChatGroupDto dto, PublicChatGroupsView view)
        {
            bool passesChecks = true;
            string errorMessage = "";

            if (dto.ChatGroupName == view.ChatGroupName)
            {
                return (false, "No change to name. No modification needed.");
            }
            if (GroupNameTaken(dto.ChatGroupName) == true)
            {
                return (false, "Chat Group name alrady taken.");
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
                    UserName         = view.UserName
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
                UserName         = view.UserName
            };
        }

        private PublicChatGroups CreateDtoToNewModel(CreatePublicChatGroupDto dto)
        {
            return new()
            {
                ChatGroupName    = dto.ChatGroupName,
                GroupCreated     = DateTime.Now,
                GroupOwnerUserId = dto.GroupOwnerUserId.ToString()
            };
        }

        public PublicChatGroups ModifyDtoToModel(ModifyPublicChatGroupDto modifyDto, PublicChatGroupsView view)
        {
            return new()
            {
                ChatGroupId      = modifyDto.ChatGroupId,
                ChatGroupName    = modifyDto.ChatGroupName,
                GroupCreated     = view.GroupCreated,
                GroupOwnerUserId = view.GroupOwnerUserId.ToString()
            };
        }

        private PublicChatGroupsDto ModelToDto(PublicChatGroups chatgroup, string userName)
        {
            return new()
            {
                ChatGroupId      = chatgroup.ChatGroupId,
                ChatGroupName    = chatgroup.ChatGroupName,
                GroupCreated     = chatgroup.GroupCreated,
                GroupOwnerUserId = Guid.Parse(chatgroup.GroupOwnerUserId),
                UserName         = userName
            };
        }

        private PublicChatGroups DtoToChatGroup(PublicChatGroupsDto dto)
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
