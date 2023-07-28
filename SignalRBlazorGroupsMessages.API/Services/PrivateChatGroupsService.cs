using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using System.Text.RegularExpressions;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public class PrivateChatGroupsService : IPrivateChatGroupsService
    {
        private readonly IPrivateChatGroupsDataAccess _privateGroupsDataAccess;
        private readonly IPrivateGroupMessagesDataAccess _privateGroupMessagesDataAccess;
        private readonly ISerilogger _serilogger;

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
                if (!await _privateGroupsDataAccess.GroupExists(groupId))
                {
                    return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RecordNotFound);
                }
                if (!await _privateGroupsDataAccess.IsUserInPrivateGroup(groupId, userId))
                {
                    return ReturnApiResponse.Failure(apiResponse, ErrorMessages.InvalidUserId);
                }

                PrivateChatGroupsDto dto = await _privateGroupsDataAccess.GetDtoByGroupIdAsync(groupId);
                return ReturnApiResponse.Success(apiResponse, dto);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.GetDtoByGroupId", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RetrievingItems);
            }
        }

        public async Task<ApiResponse<PrivateChatGroupsDto>> AddAsync(CreatePrivateChatGroupDto createDto)
        {
            ApiResponse<PrivateChatGroupsDto> apiResponse = new();

            try
            {
                if (GroupNameTaken(createDto.ChatGroupName))
                {
                    return ReturnApiResponse.Failure(apiResponse, ErrorMessages.GroupNameTaken);
                }

                PrivateChatGroups newGroup = CreateDtoToModel(createDto);

                return await _privateGroupsDataAccess.AddAsync(newGroup) ?
                    ReturnApiResponse.Success(apiResponse, await _privateGroupsDataAccess.GetDtoByGroupIdAsync(newGroup.ChatGroupId)) :
                    ReturnApiResponse.Failure(apiResponse, ErrorMessages.AddingItem);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.AddAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.AddingItem);
            }
        }

        public async Task<ApiResponse<PrivateChatGroupsDto>> ModifyAsync(ModifyPrivateChatGroupDto modifyDto, string jwtUserId)
        {
            ApiResponse<PrivateChatGroupsDto> apiResponse = new();

            try
            {
                if (!await _privateGroupsDataAccess.GroupExists(modifyDto.ChatGroupId))
                {
                    return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RecordNotFound);
                }

                PrivateChatGroups groupToModify = _privateGroupsDataAccess.GetByGroupId(modifyDto.ChatGroupId);

                (bool, string) messagechecks = ModifyChatGroupChecks(modifyDto, groupToModify, jwtUserId);
                if (messagechecks.Item1 == false)
                {
                    return ReturnApiResponse.Failure(apiResponse, messagechecks.Item2);
                }
                else
                {
                    groupToModify.ChatGroupName = modifyDto.ChatGroupName;
                }

                return await _privateGroupsDataAccess.ModifyAsync(groupToModify) ?
                    ReturnApiResponse.Success(apiResponse, await _privateGroupsDataAccess.GetDtoByGroupIdAsync(groupToModify.ChatGroupId)) :
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

        public async Task<ApiResponse<PrivateGroupMembers>> AddPrivateGroupMember(int groupId, string userId)
        {
            ApiResponse<PrivateGroupMembers> apiResponse = new();

            try
            {
                if (await _privateGroupsDataAccess.IsUserInPrivateGroup(groupId, userId))
                {
                    return ReturnApiResponse.Failure(apiResponse, ErrorMessages.UserAlreadyInGroup);
                }

                PrivateGroupMembers newMember = new()
                {
                    PrivateChatGroupId = groupId,
                    UserId = userId
                };

                return await _privateGroupsDataAccess.AddUserToGroupAsync(newMember) ? 
                    ReturnApiResponse.Success(apiResponse, newMember) :
                    ReturnApiResponse.Failure(apiResponse, ErrorMessages.AddingUser);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.AddPrivateGroupMember", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.AddingUser);
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
                    return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RecordNotFound);
                }

                return await _privateGroupsDataAccess.RemoveUserFromPrivateChatGroup(groupMember) ?
                    ReturnApiResponse.Success(apiResponse, new()) :
                    ReturnApiResponse.Failure(apiResponse, ErrorMessages.RemovingUser);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.RemoveUserFromPrivateChatGroupAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RemovingUser);
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
            if (group.GroupOwnerUserId != jwtUserId)
                return (false, ErrorMessages.InvalidUserId);
            if (modifyDto.ChatGroupName == group.ChatGroupName)
                return (false, ErrorMessages.NoModification);
            if (GroupNameTaken(modifyDto.ChatGroupName))
                return (false, ErrorMessages.GroupNameTaken);

            return (true, "");
        }

        // Return if a condition fails. If one condition fails, we do not want to continue processing
        // as this can cause additional errors.
        private async Task<(bool, string)> DeleteChatGroupChecks(PrivateChatGroups groupToDelete, string jwtUserId)
        {
            if (groupToDelete.GroupOwnerUserId != jwtUserId)
                return (false, ErrorMessages.InvalidUserId);
            if (!await _privateGroupMessagesDataAccess.DeleteAllMessagesInGroupAsync(groupToDelete.ChatGroupId))
                return (false, ErrorMessages.DeletingMessages);
            if (!await _privateGroupsDataAccess.RemoveAllUsersFromGroupAsync(groupToDelete.ChatGroupId))
                return (false, ErrorMessages.DeletingUser);

            return (true, "");
        }

        #endregion
    }
}
