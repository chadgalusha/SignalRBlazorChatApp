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
                return ReturnApiResponse.Failure(response, ErrorMessages.RetrievingItems);
            }
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> GetByIdAsync(int groupId)
        {
            ApiResponse<PublicChatGroupsDto> response = new();

            try
            {
                if (!GroupExists(groupId))
                {
                    return ReturnApiResponse.Failure(response, ErrorMessages.RecordNotFound);
                }

                PublicChatGroupsDto dtoChatGroup = await _publicGroupsDataAccess.GetDtoByIdAsync(groupId);
                return ReturnApiResponse.Success(response, dtoChatGroup);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.GetChatGroupByIdAsync", ex);
                return ReturnApiResponse.Failure(response, ErrorMessages.RetrievingItems);
            }
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> AddAsync(CreatePublicChatGroupDto createDto)
        {
            ApiResponse<PublicChatGroupsDto> apiResponse = new();

            try
            {
                if (GroupNameTaken(createDto.ChatGroupName) == true)
                {
                    return ReturnApiResponse.Failure(apiResponse, ErrorMessages.GroupNameTaken);
                }

                PublicChatGroups newChatGroup = CreateDtoToModel(createDto);

                return await _publicGroupsDataAccess.AddAsync(newChatGroup)?
                    ReturnApiResponse.Success(apiResponse, await _publicGroupsDataAccess.GetDtoByIdAsync(newChatGroup.ChatGroupId)) :
                    ReturnApiResponse.Failure(apiResponse, ErrorMessages.AddingItem);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.AddAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.AddingItem);
            }
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> ModifyAsync(ModifyPublicChatGroupDto modifyDto, string jwtUserId)
        {
            ApiResponse<PublicChatGroupsDto> apiResponse = new();

            try
            {
                PublicChatGroups groupToModify = _publicGroupsDataAccess.GetByGroupId(modifyDto.ChatGroupId);

                (bool, string) messageChecks = ModifyChatGroupChecks(modifyDto, groupToModify, jwtUserId);
                if (messageChecks.Item1 == false)
                {
                    return ReturnApiResponse.Failure(apiResponse, messageChecks.Item2);
                }
                else
                {
                    groupToModify.ChatGroupName = modifyDto.ChatGroupName;
                }

                return await _publicGroupsDataAccess.ModifyAsync(groupToModify) ?
                    ReturnApiResponse.Success(apiResponse, await _publicGroupsDataAccess.GetDtoByIdAsync(groupToModify.ChatGroupId)) :
                    ReturnApiResponse.Failure(apiResponse, ErrorMessages.ModifyingItem);
            }
            catch (InvalidOperationException ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.DeleteAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RecordNotFound);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.ModifyAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.ModifyingItem);
            }
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> DeleteAsync(int groupId, string jwtUserId)
        {
            ApiResponse<PublicChatGroupsDto> apiResponse = new();

            try
            {
                PublicChatGroups groupToDelete = _publicGroupsDataAccess.GetByGroupId(groupId);

                // delete all messages in a public group
                (bool, string) deleteChecks = await DeleteChatGroupChecks(groupToDelete, jwtUserId);
                if (deleteChecks.Item1 == false)
                {
                    return ReturnApiResponse.Failure(apiResponse, deleteChecks.Item2);
                }

                // delete the chat group
                return await _publicGroupsDataAccess.DeleteAsync(groupToDelete) ?
                    ReturnApiResponse.Success(apiResponse, new()) :
                    ReturnApiResponse.Failure(apiResponse, ErrorMessages.DeletingItem);
            }
            catch (InvalidOperationException ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.DeleteAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RecordNotFound);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.DeleteAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.DeletingItem);
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

        private (bool, string) ModifyChatGroupChecks(ModifyPublicChatGroupDto dto, PublicChatGroups group, string jwtUserId)
        {
            if (group.GroupOwnerUserId != jwtUserId)
                return (false, ErrorMessages.InvalidUserId);
            if (dto.ChatGroupName == group.ChatGroupName)
                return (false, ErrorMessages.NoModification);
            if (GroupNameTaken(dto.ChatGroupName) == true)
                return (false, ErrorMessages.GroupNameTaken);

            return (true, "");
        }

        private async Task<(bool, string)> DeleteChatGroupChecks(PublicChatGroups groupToDelete, string jwtUserId)
        {
            if (groupToDelete.GroupOwnerUserId != jwtUserId)
                return (false, ErrorMessages.InvalidUserId);
            if (!await _publicMessagesService.DeleteAllMessagesInGroupAsync(groupToDelete.ChatGroupId))
                return (false, ErrorMessages.DeletingMessages);

            return (true, "");
        }

        private PublicChatGroups CreateDtoToModel(CreatePublicChatGroupDto dto)
        {
            return new()
            {
                ChatGroupName    = dto.ChatGroupName,
                GroupCreated     = DateTime.Now,
                GroupOwnerUserId = dto.GroupOwnerUserId
            };
        }

        #endregion
    }
}
