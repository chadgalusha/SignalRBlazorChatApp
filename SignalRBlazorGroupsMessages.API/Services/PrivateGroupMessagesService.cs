using ChatApplicationModels;
using Microsoft.IdentityModel.Tokens;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public class PrivateGroupMessagesService : IPrivateGroupMessagesService
    {
        private readonly IPrivateGroupMessagesDataAccess _privateMessageDataAccess;
        private readonly ISerilogger _serilogger;

        public PrivateGroupMessagesService(IPrivateGroupMessagesDataAccess privateMessageDataAccess, ISerilogger serilogger)
        {
            _privateMessageDataAccess = privateMessageDataAccess ?? throw new Exception(nameof(privateMessageDataAccess));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        public async Task<ApiResponse<List<PrivateGroupMessageDto>>> GetDtoListByGroupIdAsync(int groupId, int numberMessagesToSkip)
        {
            ApiResponse<List<PrivateGroupMessageDto>> apiResponse = new();

            try
            {
                if (!await _privateMessageDataAccess.GroupIdExists(groupId))
                {
                    return ReturnApiResponse.Failure(apiResponse, "Group Id does not exists.");
                }

                List<PrivateGroupMessageDto> dtoList = await _privateMessageDataAccess.GetDtoListByGroupIdAsync(groupId, numberMessagesToSkip);
                return ReturnApiResponse.Success(apiResponse, dtoList);
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.GetListByGroupIdAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, "Error getting messages.");
            }
        }

        public async Task<ApiResponse<List<PrivateGroupMessageDto>>> GetDtoListByUserIdAsync(string userId, int numberMessagesToSkip)
        {
            ApiResponse<List<PrivateGroupMessageDto>> apiResponse = new();

            try
            {
                List<PrivateGroupMessageDto> dtoList = await _privateMessageDataAccess.GetDtoListByUserIdAsync(userId, numberMessagesToSkip);
                return ReturnApiResponse.Success(apiResponse, dtoList);
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.GetDtoListByUserIdAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, "Error getting messages.");
            }
        }

        public async Task<ApiResponse<PrivateGroupMessageDto>> GetDtoByMessageIdAsync(Guid messageId)
        {
            ApiResponse<PrivateGroupMessageDto> apiResponse = new();

            try
            {
                PrivateGroupMessageDto dtoMessage = await _privateMessageDataAccess.GetDtoByMessageIdAsync(messageId);

                if (messageId == new Guid() || dtoMessage.PrivateMessageId == new Guid())
                {
                    return ReturnApiResponse.Failure(apiResponse, "Message Id not found.");
                }
                
                return ReturnApiResponse.Success(apiResponse, dtoMessage);
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.GetDtoByMessageIdAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, "Error getting message.");
            }
        }

        public async Task<ApiResponse<PrivateGroupMessageDto>> AddAsync(CreatePrivateGroupMessageDto createDto)
        {
            ApiResponse<PrivateGroupMessageDto> apiResponse = new();

            (bool, string) messagechecks = NewMessageChecks(createDto);
            if (messagechecks.Item1 == false)
            {
                apiResponse.Data = null;
                return ReturnApiResponse.Failure(apiResponse, messagechecks.Item2);
            }

            try
            {
                PrivateGroupMessages newMessage = NewModel(createDto);
                bool isSuccess = await _privateMessageDataAccess.AddAsync(newMessage);

                if (!isSuccess)
                {
                    return ReturnApiResponse.Failure(apiResponse, "Error saving new message.");
                }

                return ReturnApiResponse.Success(apiResponse, 
                    await _privateMessageDataAccess.GetDtoByMessageIdAsync(newMessage.PrivateMessageId));
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.AddAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, "Error saving new message.");
            }
        }

        public async Task<ApiResponse<PrivateGroupMessageDto>> ModifyAsync(ModifyPrivateGroupMessageDto modifyDto, string jwtUserId)
        {
            ApiResponse<PrivateGroupMessageDto> apiResponse = new();

            try
            {
                PrivateGroupMessages messageToModify = await _privateMessageDataAccess.GetByMessageIdAsync(modifyDto.PrivateMessageId);

                // if message not in db or userIds do not match
                (bool, string) messageChecks = Messagechecks(messageToModify.PrivateMessageId, messageToModify.UserId, jwtUserId);
                if (messageChecks.Item1 == false)
                {
                    return ReturnApiResponse.Failure(apiResponse, messageChecks.Item2);
                }
                
                messageToModify = ModifyDtoToModel(modifyDto, messageToModify);
                if (!await _privateMessageDataAccess.ModifyAsync(messageToModify))
                {
                    return ReturnApiResponse.Failure(apiResponse, "Error modifying message.");
                }

                return ReturnApiResponse.Success(apiResponse, 
                    await _privateMessageDataAccess.GetDtoByMessageIdAsync(messageToModify.PrivateMessageId));
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.ModifyAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, "Error modifying message.");
            }
        }

        public async Task<ApiResponse<PrivateGroupMessageDto>> DeleteAsync(Guid messageId, string jwtUserId)
        {
            ApiResponse<PrivateGroupMessageDto> apiResponse = new();

            try
            {
                // Find message to delete
                PrivateGroupMessages messageToDelete = await _privateMessageDataAccess.GetByMessageIdAsync(messageId);

                // if message not in db or userIds do not match
                (bool, string) messageChecks = Messagechecks(messageToDelete.PrivateMessageId, messageToDelete.UserId, jwtUserId);
                if (messageChecks.Item1 == false)
                {
                    return ReturnApiResponse.Failure(apiResponse, messageChecks.Item2);
                }

                // delete all messages that are a response to this message
                bool responseMessagesDeleted = await _privateMessageDataAccess.DeleteMessagesByReplyMessageIdAsync(messageId);
                if (!responseMessagesDeleted)
                {
                    return ReturnApiResponse.Failure(apiResponse, "Response messages not deleted.");
                }

                // delete the message
                bool isSuccess = await _privateMessageDataAccess.DeleteAsync(messageToDelete);
                if (!isSuccess)
                {
                    return ReturnApiResponse.Failure(apiResponse, "Error deleting message.");
                }

                return ReturnApiResponse.Success(apiResponse, new());
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.DeleteAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, "Error deleting message.");
            }
        }

        public async Task<bool> DeleteAllMessagesInGroupAsync(int groupId)
        {
            return await _privateMessageDataAccess.DeleteAllMessagesInGroupAsync(groupId);
        }

        #region PRIVATE METHODS

        private (bool, string) NewMessageChecks(CreatePrivateGroupMessageDto createDto)
        {
            bool passesChecks = true;
            string errorMessage = "";

            if (createDto.UserId.IsNullOrEmpty() || createDto.UserId == new Guid().ToString())
            {
                passesChecks = false;
                errorMessage += "[Invalid userId]";
            }
            if (createDto.Text.IsNullOrEmpty())
            {
                passesChecks = false;
                errorMessage += "[Text is empty or null]";
            }

            return (passesChecks, errorMessage);
        }

        private (bool, string) Messagechecks(Guid messageId, string messageUserId, string jwtUserId)
        {
            bool result = true;
            string message = "";

            if (messageId == new Guid())
            {
                result = false;
                message += "[Message Id not found.]";
            }
            if (messageUserId != jwtUserId)
            {
                result = false;
                message += "[Requesting userId not valid for this request.]";
            }

            return (result, message);
        }

        private PrivateGroupMessages NewModel(CreatePrivateGroupMessageDto createDto)
        {
            return new()
            {
                PrivateMessageId = Guid.NewGuid(),
                UserId = createDto.UserId,
                ChatGroupId = createDto.ChatGroupId,
                Text = createDto.Text,
                MessageDateTime = DateTime.Now,
                ReplyMessageId = createDto.ReplyMessageId,
                PictureLink = createDto.PictureLink,
            };
        }

        // retain messageId, userId, chatGroupId, and messageDateTime from original message. only modify text and replymessageid and/or picturelink.
        private PrivateGroupMessages ModifyDtoToModel(ModifyPrivateGroupMessageDto modifyDto, PrivateGroupMessages message)
        {
            return new()
            {
                PrivateMessageId = message.PrivateMessageId,
                UserId = message.UserId,
                ChatGroupId = message.ChatGroupId,
                Text = modifyDto.Text,
                MessageDateTime = message.MessageDateTime,
                ReplyMessageId = modifyDto.ReplyMessageId,
                PictureLink = modifyDto.PictureLink
            };
        }

        #endregion
    }
}
