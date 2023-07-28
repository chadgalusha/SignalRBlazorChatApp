using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public class PublicChatGroupsService : IPublicChatGroupsService
    {
        private readonly IPublicChatGroupsDataAccess _publicGroupsDataAccess;
        private readonly IPublicGroupMessagesService _publicMessagesService;
        private readonly ISerilogger _serilogger;

        public PublicChatGroupsService(IPublicChatGroupsDataAccess publicGroupsDataAccess, IPublicGroupMessagesService publicMessagesService, ISerilogger serilogger)
        {
            _publicGroupsDataAccess = publicGroupsDataAccess ?? throw new Exception(nameof(publicGroupsDataAccess));
            _publicMessagesService = publicMessagesService ?? throw new Exception(nameof(publicMessagesService));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        public async Task<ApiResponse<List<PublicChatGroupsDto>>> GetListAsync()
        {
            ApiResponse<List<PublicChatGroupsDto>> response = new();

            try
            {
                List<PublicChatGroupsDto> dtoList = await _publicGroupsDataAccess.GetDtoListAsync();
                return ReturnApiResponse.Success(response, dtoList);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.ChatGroupsService", ex);
                return ReturnApiResponse.Failure(response, "Error getting public chat groups.");
            }
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> GetByIdAsync(int groupId)
        {
            ApiResponse<PublicChatGroupsDto> response = new();

            try
            {
                if (!GroupExists(groupId))
                {
                    return ReturnApiResponse.Failure(response, "invalid groupId");
                }

                PublicChatGroupsDto dtoChatGroup = await _publicGroupsDataAccess.GetDtoByIdAsync(groupId);

                return ReturnApiResponse.Success(response, dtoChatGroup);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.GetChatGroupByIdAsync", ex);
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
                    return ReturnApiResponse.Failure(response, "Chat Group name already taken.");
                }

                PublicChatGroups newChatGroup = CreateDtoToNewModel(dto);

                if (!await _publicGroupsDataAccess.AddAsync(newChatGroup))
                {
                    return ReturnApiResponse.Failure(response, "Error creating new chat group.");
                }

                return ReturnApiResponse.Success(response, 
                    await _publicGroupsDataAccess.GetDtoByIdAsync(newChatGroup.ChatGroupId));
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.AddAsync", ex);
                return ReturnApiResponse.Failure(response, "Error creating new chat group.");
            }
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> ModifyAsync(ModifyPublicChatGroupDto dtoToModify, string jwtUserId)
        {
            ApiResponse<PublicChatGroupsDto> response = new();

            try
            {
                PublicChatGroupsDto existingDto = await _publicGroupsDataAccess.GetDtoByIdAsync(dtoToModify.ChatGroupId);

                // check if chat group passes modify checks 1st. if false return failure with error messages.
                (bool, string) messageChecks = ModifyChatGroupChecks(dtoToModify, existingDto);
                if (messageChecks.Item1 == false)
                {
                    return ReturnApiResponse.Failure(response, messageChecks.Item2);
                }

                PublicChatGroups chatGroupToModify = ModifyDtoToModel(dtoToModify, existingDto);
                bool isSuccess = await _publicGroupsDataAccess.ModifyAsync(chatGroupToModify);

                if (!isSuccess)
                {
                    return ReturnApiResponse.Failure(response, "Error modifying chat group.");
                }

                return ReturnApiResponse.Success(response, ModelToDto(chatGroupToModify, existingDto.UserName));
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.ModifyAsync", ex);
                return ReturnApiResponse.Failure(response, "Error modifying chat group.");
            }
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> DeleteAsync(int groupId, string jwtUserId)
        {
            ApiResponse<PublicChatGroupsDto> response = new();

            try
            {
                if (!_publicGroupsDataAccess.GroupExists(groupId))
                {
                    return ReturnApiResponse.Failure(response, "Chat Group Id not found");
                }

                // delete all messages in a public group
                bool deleteMessagesResult = await _publicMessagesService.DeleteAllMessagesInGroupAsync(groupId);
                if (!deleteMessagesResult)
                {
                    return ReturnApiResponse.Failure(response, "Error deleting messages from this group");
                }

                // get group model to delete, assign to dto for return data
                PublicChatGroups groupToDelete = _publicGroupsDataAccess.GetByGroupId(groupId);
                PublicChatGroupsDto dtoToReturn = ModelToDto(groupToDelete, "");

                // delete the chat group
                bool deleteChatGroupResponse = await _publicGroupsDataAccess.DeleteAsync(groupToDelete);
                if (!deleteChatGroupResponse)
                {
                    return ReturnApiResponse.Failure(response, "Error deleting the chat group.");
                }

                return ReturnApiResponse.Success(response, dtoToReturn);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.DeleteAsync", ex);
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

        private (bool, string) ModifyChatGroupChecks(ModifyPublicChatGroupDto dto, PublicChatGroupsDto view)
        {
            bool passesChecks = true;
            string errorMessage = "";

            if (dto.ChatGroupName == view.ChatGroupName)
            {
                passesChecks = false;
                errorMessage += "[No change to name. No modification needed.]";
            }
            if (GroupNameTaken(dto.ChatGroupName) == true)
            {
                passesChecks = false;
                errorMessage += "[Chat Group name alrady taken.]";
            }

            return (passesChecks, errorMessage);
        }

        //private List<PublicChatGroupsDto> ViewListToDtoList(List<PublicChatGroupsView> viewList)
        //{
        //    List<PublicChatGroupsDto> dtoList = new();

        //    foreach (PublicChatGroupsView view in viewList)
        //    {
        //        PublicChatGroupsDto dto = new()
        //        {
        //            ChatGroupId      = view.ChatGroupId,
        //            ChatGroupName    = view.ChatGroupName,
        //            GroupCreated     = view.GroupCreated,
        //            GroupOwnerUserId = view.GroupOwnerUserId,
        //            UserName         = view.UserName
        //        };
        //        dtoList.Add(dto);
        //    }
        //    return dtoList;
        //}

        //private PublicChatGroupsDto ViewToDto(PublicChatGroupsView view)
        //{
        //    return new()
        //    {
        //        ChatGroupId      = view.ChatGroupId,
        //        ChatGroupName    = view.ChatGroupName,
        //        GroupCreated     = view.GroupCreated,
        //        GroupOwnerUserId = view.GroupOwnerUserId,
        //        UserName         = view.UserName
        //    };
        //}

        private PublicChatGroups CreateDtoToNewModel(CreatePublicChatGroupDto dto)
        {
            return new()
            {
                ChatGroupName    = dto.ChatGroupName,
                GroupCreated     = DateTime.Now,
                GroupOwnerUserId = dto.GroupOwnerUserId.ToString()
            };
        }

        public PublicChatGroups ModifyDtoToModel(ModifyPublicChatGroupDto modifyDto, PublicChatGroupsDto dto)
        {
            return new()
            {
                ChatGroupId      = modifyDto.ChatGroupId,
                ChatGroupName    = modifyDto.ChatGroupName,
                GroupCreated     = dto.GroupCreated,
                GroupOwnerUserId = dto.GroupOwnerUserId.ToString()
            };
        }

        private PublicChatGroupsDto ModelToDto(PublicChatGroups chatgroup, string userName)
        {
            return new()
            {
                ChatGroupId      = chatgroup.ChatGroupId,
                ChatGroupName    = chatgroup.ChatGroupName,
                GroupCreated     = chatgroup.GroupCreated,
                GroupOwnerUserId = chatgroup.GroupOwnerUserId,
                UserName         = userName
            };
        }

        private PublicChatGroups DtoToChatGroup(PublicChatGroupsDto dto, string jwtUserId)
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
