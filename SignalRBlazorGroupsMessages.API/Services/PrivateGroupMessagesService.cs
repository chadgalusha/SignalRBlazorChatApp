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
                    return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RecordNotFound);
                }

                List<PrivateGroupMessageDto> dtoList = await _privateMessageDataAccess.GetDtoListByGroupIdAsync(groupId, numberMessagesToSkip);
                return ReturnApiResponse.Success(apiResponse, dtoList);
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.GetListByGroupIdAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RetrievingItems);
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
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RetrievingItems);
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
                    return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RecordNotFound);
                }
                
                return ReturnApiResponse.Success(apiResponse, dtoMessage);
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.GetDtoByMessageIdAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RetrievingItems);
            }
        }

        public async Task<ApiResponse<PrivateGroupMessageDto>> AddAsync(CreatePrivateGroupMessageDto createDto)
        {
            ApiResponse<PrivateGroupMessageDto> apiResponse = new();

            try
            {
                PrivateGroupMessages newMessage = NewModel(createDto);

                return await _privateMessageDataAccess.AddAsync(newMessage) ?
                    ReturnApiResponse.Success(apiResponse,await _privateMessageDataAccess.GetDtoByMessageIdAsync(newMessage.PrivateMessageId)) :
                    ReturnApiResponse.Failure(apiResponse, ErrorMessages.RetrievingItems);
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.AddAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RetrievingItems);
            }
        }

        public async Task<ApiResponse<PrivateGroupMessageDto>> ModifyAsync(ModifyPrivateGroupMessageDto modifyDto, string jwtUserId)
        {
            ApiResponse<PrivateGroupMessageDto> apiResponse = new();

            try
            {
                PrivateGroupMessages messageToModify = await _privateMessageDataAccess.GetByMessageIdAsync(modifyDto.PrivateMessageId);

                (bool, string) modifyMessageCheck = ModifyMessagechecks(modifyDto, messageToModify, jwtUserId);
                if (modifyMessageCheck.Item1 == false)
                {
                    return ReturnApiResponse.Failure(apiResponse, modifyMessageCheck.Item2);
                }
                
                messageToModify = ModifyDtoToModel(modifyDto, messageToModify);

                return await _privateMessageDataAccess.ModifyAsync(messageToModify) ?
                    ReturnApiResponse.Success(apiResponse, await _privateMessageDataAccess.GetDtoByMessageIdAsync(messageToModify.PrivateMessageId)) :
                    ReturnApiResponse.Failure(apiResponse, ErrorMessages.RetrievingItems);
            }
            catch (InvalidOperationException ex)
            {
                _serilogger.ChatGroupError("PrivateMessagesService.ModifyAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RecordNotFound);
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.ModifyAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RetrievingItems);
            }
        }

        public async Task<ApiResponse<PrivateGroupMessageDto>> DeleteAsync(Guid messageId, string jwtUserId)
        {
            ApiResponse<PrivateGroupMessageDto> apiResponse = new();

            try
            {
                PrivateGroupMessages messageToDelete = await _privateMessageDataAccess.GetByMessageIdAsync(messageId);

                // if message not in db or userIds do not match
                (bool, string) deleteMessageChecks = DeleteMessageChecks(messageToDelete, jwtUserId);
                if (deleteMessageChecks.Item1 == false)
                {
                    return ReturnApiResponse.Failure(apiResponse, deleteMessageChecks.Item2);
                }

                // delete all messages that are a response to this message
                bool responseMessagesDeleted = await _privateMessageDataAccess.DeleteMessagesByReplyMessageIdAsync(messageId);
                if (!responseMessagesDeleted)
                {
                    return ReturnApiResponse.Failure(apiResponse, "Response messages not deleted.");
                }

                return await _privateMessageDataAccess.DeleteAsync(messageToDelete) ?
                    ReturnApiResponse.Success(apiResponse, new()) :
                    ReturnApiResponse.Failure(apiResponse, ErrorMessages.DeletingItem);
            }
            catch (InvalidOperationException ex)
            {
                _serilogger.ChatGroupError("PrivateMessagesService.DeleteAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RecordNotFound);
            }
            catch (Exception ex)
            {
                _serilogger.PrivateMessageError("PrivateMessagesService.DeleteAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.DeletingItem);
            }
        }

        public async Task<bool> DeleteAllMessagesInGroupAsync(int groupId)
        {
            return await _privateMessageDataAccess.DeleteAllMessagesInGroupAsync(groupId);
        }

        #region PRIVATE METHODS

        private (bool, string) ModifyMessagechecks(ModifyPrivateGroupMessageDto dto, PrivateGroupMessages message, string jwtUserId)
        {

            if (message.UserId != jwtUserId)
                return (false, ErrorMessages.InvalidUserId);
            if (dto.Text == message.Text
                && dto.ReplyMessageId == message.ReplyMessageId
                && dto.PictureLink == message.PictureLink)
                return (false, ErrorMessages.NoModification);
            return (true, "");
        }

        private (bool, string) DeleteMessageChecks()
        {
            // TODO message checks

            return (true, "");
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
