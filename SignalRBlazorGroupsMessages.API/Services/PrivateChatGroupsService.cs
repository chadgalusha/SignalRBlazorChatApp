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

        public async Task<ApiResponse<List<PrivateChatGroupsDto>>> GetDtoListByUserId(string userId)
        {
            ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new();

            try
            {
                List<PrivateChatGroupsDto> dtoList = await _privateGroupsDataAccess.GetDtoListByUserIdAsync(userId);
                return ReturnApiResponse.Success(apiResponse, dtoList);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.GetDtoListByUserId", ex);
                return ReturnApiResponse.Failure(apiResponse, "Error getting private chat groups.");
            }
        }

        public async Task<ApiResponse<PrivateChatGroupsDto>> GetDtoByGroupId(int groupId, string userId)
        {
            ApiResponse<PrivateChatGroupsDto> apiResponse = new();

            try
            {
                if (!await _privateGroupsDataAccess.IsUserInPrivateGroup(groupId, userId))
                {
                    return ReturnApiResponse.Failure(apiResponse, "Requesting userId not valid for this request.");
                }

                PrivateChatGroupsDto dto = await _privateGroupsDataAccess.GetDtoByGroupIdAsync(groupId);
                return ReturnApiResponse.Success(apiResponse, dto);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.GetDtoByGroupId", ex);
                return ReturnApiResponse.Failure(apiResponse, "Error getting private chat group.");
            }
        }

        public async Task<ApiResponse<PrivateChatGroupsDto>> AddAsync(CreatePrivateChatGroupDto createDto)
        {
            ApiResponse<PrivateChatGroupsDto> apiResponse = new();

            try
            {
                if (GroupNameTaken(createDto.ChatGroupName))
                {
                    return ReturnApiResponse.Failure(apiResponse, "Chat Group name already taken.");
                }

                PrivateChatGroups newGroup = CreateDtoToModel(createDto);

                return await _privateGroupsDataAccess.AddAsync(newGroup) ?
                    ReturnApiResponse.Success(apiResponse, await _privateGroupsDataAccess.GetDtoByGroupIdAsync(newGroup.ChatGroupId)) :
                    ReturnApiResponse.Failure(apiResponse, "Error saving private chat group.");
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.AddAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, "Error saving private chat group.");
            }
        }

        public async Task<ApiResponse<PrivateChatGroupsDto>> ModifyAsync(ModifyPrivateChatGroupDto modifyDto)
        {
            ApiResponse<PrivateChatGroupsDto> apiResponse = new();

            try
            {
                PrivateChatGroups groupToModify = _privateGroupsDataAccess.GetByGroupId(modifyDto.ChatGroupId);

                (bool, string) messagechecks = ModifyChatGroupChecks(modifyDto, groupToModify);
                if (messagechecks.Item1 == false)
                {
                    return ReturnApiResponse.Failure(apiResponse, messagechecks.Item2);
                }

                return await _privateGroupsDataAccess.ModifyAsync(groupToModify) ?
                    ReturnApiResponse.Success(apiResponse, await _privateGroupsDataAccess.GetDtoByGroupIdAsync(groupToModify.ChatGroupId)) :
                    ReturnApiResponse.Failure(apiResponse, "Error modifying private chat group.");
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.ModifyAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, "Error modifying private chat group.");
            }
        }

        public async Task<ApiResponse<PrivateChatGroupsDto>> DeleteAsync(int groupId, string userId)
        {
            ApiResponse<PrivateChatGroupsDto> apiResponse = new();

            try
            {
                PrivateChatGroups groupToDelete = _privateGroupsDataAccess.GetByGroupId(groupId);

                // TODO: verify userid is groupownerid, if not return error.

                // TODO: delete all messages from group

                // TODO: remove all users from group access

                return await _privateGroupsDataAccess.DeleteAsync(groupToDelete) ?
                    ReturnApiResponse.Success(apiResponse, new()) :
                    ReturnApiResponse.Failure(apiResponse, "Error deleting private chat group.");
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.DeleteAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, "Error deleting private chat group.");
            }
        }

        #region PRIVATE METHODS

        private PrivateChatGroups CreateDtoToModel(CreatePrivateChatGroupDto createDto)
        {
            return new()
            {
                ChatGroupName    = createDto.ChatGroupName,
                GroupCreated     = DateTime.Now,
                GroupOwnerUserId = createDto.GroupOwnerUserId.ToString()
            };
        }

        private bool GroupNameTaken(string groupName) 
        {
            return _privateGroupsDataAccess.GroupNameTaken(groupName);
        }

        private (bool, string) ModifyChatGroupChecks(ModifyPrivateChatGroupDto modifyDto, PrivateChatGroups group)
        {
            bool passesChecks = true;
            string errorMessage = "";

            if (modifyDto.ChatGroupName == group.ChatGroupName)
            {
                passesChecks = false;
                errorMessage += "[No change to name. No modification needed.]";
            }
            if (GroupNameTaken(modifyDto.ChatGroupName))
            {
                passesChecks = false;
                errorMessage += "[Chat Group name alrady taken.]";
            }

            return (passesChecks, errorMessage);
        }

        #endregion
    }
}
