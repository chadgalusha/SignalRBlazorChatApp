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

                apiResponse.Data = null;
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

                apiResponse.Data = null;
                return ReturnApiResponse.Failure(apiResponse, "Error getting messages.");
            }
        }

        public async Task<ApiResponse<PrivateGroupMessageDto>> GetDtoByMessageIdAsync(Guid messageId)
        {
            ApiResponse<PrivateGroupMessageDto> apiResponse = new();

            try
            {
                if (messageId == new Guid())
                {
                    apiResponse.Data = null;
                    return ReturnApiResponse.Failure(apiResponse, "Invalid messageId.");
                }

                PrivateGroupMessageDto dtoMessage = await _privateMessageDataAccess.GetDtoByMessageIdAsync(messageId);
                return ReturnApiResponse.Success(apiResponse, dtoMessage);
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.GetDtoByMessageIdAsync", ex);

                apiResponse.Data = null;
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
                    apiResponse.Data = null;
                    return ReturnApiResponse.Failure(apiResponse, "Error saving new message.");
                }

                PrivateGroupMessageDto newDto = ModelToDto(newMessage);

                return ReturnApiResponse.Success(apiResponse, newDto);
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.AddAsync", ex);

                apiResponse.Data = null;
                return ReturnApiResponse.Failure(apiResponse, "Error saving new message.");
            }
        }

        public async Task<ApiResponse<PrivateGroupMessageDto>> ModifyAsync(ModifyPrivateGroupMessageDto modifyDto)
        {
            ApiResponse<PrivateGroupMessageDto> apiResponse = new();

            try
            {
                // Check message exists before proceeding
                if (await _privateMessageDataAccess.MessageIdExists(modifyDto.PrivateMessageId) == false)
                {
                    apiResponse.Data = null;
                    return ReturnApiResponse.Failure(apiResponse, "Message Id not found.");
                }

                PrivateGroupMessages messageToModify = await _privateMessageDataAccess.GetByMessageIdAsync(modifyDto.PrivateMessageId);
                messageToModify = ModifyDtoToModel(modifyDto, messageToModify);

                bool isSuccess = await _privateMessageDataAccess.ModifyAsync(messageToModify);

                if (!isSuccess)
                {
                    apiResponse.Data = null;
                    return ReturnApiResponse.Failure(apiResponse, "Error modifying message.");
                }

                PrivateGroupMessageDto returnDto = ModelToDto(messageToModify);

                return ReturnApiResponse.Success(apiResponse, returnDto);
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.ModifyAsync", ex);

                apiResponse.Data = null;
                return ReturnApiResponse.Failure(apiResponse, "Error modifying message.");
            }
        }

        public async Task<ApiResponse<PrivateGroupMessageDto>> DeleteAsync(Guid messageId)
        {
            ApiResponse<PrivateGroupMessageDto> apiResponse = new();

            try
            {
                // check message exists
                if (!await _privateMessageDataAccess.MessageIdExists(messageId))
                {
                    apiResponse.Data = null;
                    return ReturnApiResponse.Failure(apiResponse, "Message Id not found.");
                }

                // Find message to delete
                PrivateGroupMessages messageToDelete = await _privateMessageDataAccess.GetByMessageIdAsync(messageId);

                // delete all messages that are a response to this message
                bool responseMessagesDeleted = await _privateMessageDataAccess.DeleteMessagesByReplyMessageIdAsync(messageId);
                if (!responseMessagesDeleted)
                {
                    apiResponse.Data = null;
                    return ReturnApiResponse.Failure(apiResponse, "Response messages not deleted.");
                }

                // delete the message
                bool isSuccess = await _privateMessageDataAccess.DeleteAsync(messageToDelete);
                if (!isSuccess)
                {
                    apiResponse.Data = null;
                    return ReturnApiResponse.Failure(apiResponse, "Error deleting message.");
                }

                return ReturnApiResponse.Success(apiResponse, ModelToDto(messageToDelete));
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.DeleteAsync", ex);

                apiResponse.Data = null;
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

        private PrivateGroupMessageDto ModelToDto(PrivateGroupMessages message)
        {
            return new()
            {
                PrivateMessageId = message.PrivateMessageId,
                UserId = message.UserId,
                ChatGroupId = message.ChatGroupId,
                Text = message.Text,
                MessageDateTime = message.MessageDateTime,
                ReplyMessageId = message.ReplyMessageId,
                PictureLink = message.PictureLink,
            };
        }

        private PrivateGroupMessageDto ModelToDto(PrivateGroupMessages message, PrivateGroupMessageDto dto)
        {
            return new()
            {
                PrivateMessageId = message.PrivateMessageId,
                UserId = message.UserId,
                UserName = dto.UserName,
                ChatGroupId = message.ChatGroupId,
                ChatGroupName = dto.ChatGroupName,
                Text = message.Text,
                MessageDateTime = message.MessageDateTime,
                ReplyMessageId = message.ReplyMessageId,
                PictureLink = message.PictureLink,
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
