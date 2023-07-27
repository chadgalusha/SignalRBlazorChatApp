using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public class PrivateChatGroupsService : IPrivateChatGroupsService
    {
        private readonly IPrivateChatGroupsDataAccess _privateGroupsDataAccess;
        private readonly IPrivateGroupMessagesDataAccess _privateGroupMessagesDataAccess;
        private readonly ISerilogger _serilogger;
        private readonly ErrorMessages errorMessages;

        public PrivateChatGroupsService(IPrivateChatGroupsDataAccess privateGroupsDataAccess,
            IPrivateGroupMessagesDataAccess privateGroupMessagesDataAccess, ISerilogger serilogger)
        {
            _privateGroupsDataAccess = privateGroupsDataAccess ?? throw new Exception(nameof(privateGroupsDataAccess));
            _privateGroupMessagesDataAccess = privateGroupMessagesDataAccess ?? throw new Exception(nameof(privateGroupMessagesDataAccess));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        public async Task<ApiResponse<List<PrivateChatGroupsDto>>> GetDtoListByUserIdAsync(string userId)
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
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RetrievingItems);
            }
        }

        public async Task<ApiResponse<PrivateChatGroupsDto>> GetDtoByGroupIdAsync(int groupId, string userId)
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

        public async Task<ApiResponse<PrivateChatGroupsDto>> ModifyAsync(ModifyPrivateChatGroupDto modifyDto, string jwtUserId)
        {
            ApiResponse<PrivateChatGroupsDto> apiResponse = new();

            try
            {
                PrivateChatGroups groupToModify = _privateGroupsDataAccess.GetByGroupId(modifyDto.ChatGroupId);

                (bool, string) messagechecks = ModifyChatGroupChecks(modifyDto, groupToModify, jwtUserId);
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

        public async Task<ApiResponse<PrivateChatGroupsDto>> DeleteAsync(int groupId, string jwtUserId)
        {
            ApiResponse<PrivateChatGroupsDto> apiResponse = new();

            try
            {
                PrivateChatGroups groupToDelete = _privateGroupsDataAccess.GetByGroupId(groupId);

                (bool, string) passesChecks = await DeleteChatGroupChecks(groupToDelete, jwtUserId);
                if (passesChecks.Item1 == false)
                {
                    return ReturnApiResponse.Failure(apiResponse, passesChecks.Item2);
                }

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

        public async Task<ApiResponse<PrivateGroupMembers>> AddPrivateGroupMember(int groupId, string userId)
        {
            ApiResponse<PrivateGroupMembers> apiResponse = new();

            try
            {
                if (await _privateGroupsDataAccess.IsUserInPrivateGroup(groupId, userId))
                {
                    return ReturnApiResponse.Failure(apiResponse, "User is already in the private group.");
                }

                PrivateGroupMembers newMember = new()
                {
                    PrivateChatGroupId = groupId,
                    UserId = userId
                };

                bool result = await _privateGroupsDataAccess.AddUserToGroupAsync(newMember);

                return result ? ReturnApiResponse.Success(apiResponse, newMember) :
                    ReturnApiResponse.Failure(apiResponse, "Error adding new private group member.");
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.AddPrivateGroupMember", ex);
                return ReturnApiResponse.Failure(apiResponse, "Error adding new private group member.");
            }
        }

        public async Task<ApiResponse<PrivateGroupMembers>> RemoveUserFromGroupAsync(int groupId, string userId)
        {
            ApiResponse<PrivateGroupMembers> apiResponse = new();

            try
            {
                PrivateGroupMembers groupMember = await _privateGroupsDataAccess.GetPrivateGroupMemberRecord(groupId, userId);

                if (groupMember == null)
                {
                    return ReturnApiResponse.Failure(apiResponse, "Group member record not found.");
                }

                return await _privateGroupsDataAccess.RemoveUserFromPrivateChatGroup(groupMember) ?
                    ReturnApiResponse.Success(apiResponse, new()) :
                    ReturnApiResponse.Failure(apiResponse, "Error removing private group member.");
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.RemoveUserFromPrivateChatGroupAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, "Error removing private group member.");
            }
        }

        #region PRIVATE METHODS

        private PrivateChatGroups CreateDtoToModel(CreatePrivateChatGroupDto createDto)
        {
            return new()
            {
                ChatGroupName = createDto.ChatGroupName,
                GroupCreated = DateTime.Now,
                GroupOwnerUserId = createDto.GroupOwnerUserId.ToString()
            };
        }

        private bool GroupNameTaken(string groupName)
        {
            return _privateGroupsDataAccess.GroupNameTaken(groupName);
        }

        private (bool, string) ModifyChatGroupChecks(ModifyPrivateChatGroupDto modifyDto, PrivateChatGroups group, string jwtUserId)
        {
            bool passesChecks = true;
            string errorMessage = "";

            if (group.GroupOwnerUserId != jwtUserId)
            {
                passesChecks = false;
                errorMessage += "[Requesting userId not valid for this request.]";
            }
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

        // Return if a condition fails. If one condition fails, we do not want to continue processing
        // as this can cause additional errors.
        private async Task<(bool, string)> DeleteChatGroupChecks(PrivateChatGroups groupToDelete, string jwtUserId)
        {
            bool passesChecks = true;
            string errorMessage = "";

            if (groupToDelete.GroupOwnerUserId != jwtUserId)
            {
                return (false, "Requesting userId not valid for this request.");
            }

            if (!await _privateGroupMessagesDataAccess.DeleteAllMessagesInGroupAsync(groupToDelete.ChatGroupId))
            {
                return (false, "Error deleting messages from this group.");
            }

            if (!await _privateGroupsDataAccess.RemoveAllUsersFromGroupAsync(groupToDelete.ChatGroupId))
            {
                return (false, "Error removing group members from this group.");
            }

            return (passesChecks, errorMessage);
        }

        #endregion
    }
}
