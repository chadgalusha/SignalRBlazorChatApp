using ChatApplicationModels;
using Microsoft.IdentityModel.Tokens;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public class PublicGroupMessagesService : IPublicGroupMessagesService
    {
        private readonly IPublicGroupMessagesDataAccess _publicMessageDataAccess;
        private readonly ISerilogger _serilogger;

        public PublicGroupMessagesService(IPublicGroupMessagesDataAccess publicMessageDataAccess, ISerilogger serilogger)
        {
            _publicMessageDataAccess = publicMessageDataAccess ?? throw new Exception(nameof(publicMessageDataAccess));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        public async Task<ApiResponse<List<PublicGroupMessageDto>>> GetListByGroupIdAsync(int groupId, int numberItemsToSkip)
        {
            ApiResponse<List<PublicGroupMessageDto>> response = new();

            try
            {
                List<PublicGroupMessageDto> dtoList = await _publicMessageDataAccess.GetDtoListByGroupIdAsync(groupId, numberItemsToSkip);
                return ReturnApiResponse.Success(response, dtoList);
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.GetByGroupIdAsync", ex);
                return ReturnApiResponse.Failure(response, "Error getting messages.");
            }
        }

        public async Task<ApiResponse<List<PublicGroupMessageDto>>> GetListByUserIdAsync(string userId, int numberItemsToSkip)
        {
            ApiResponse<List<PublicGroupMessageDto>> response = new();

            try
            {
                List<PublicGroupMessageDto> listDto = await _publicMessageDataAccess.GetDtoListByUserIdAsync(userId, numberItemsToSkip);
                return ReturnApiResponse.Success(response, listDto);
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.GetByUserIdAsync", ex);
                return ReturnApiResponse.Failure(response, "Error getting messages.");
            }
        }

        public async Task<ApiResponse<PublicGroupMessageDto>> GetByMessageIdAsync(Guid messageId)
        {
            ApiResponse<PublicGroupMessageDto> response = new();

            try
            {
                PublicGroupMessageDto messageDto = await _publicMessageDataAccess.GetDtoByMessageIdAsync(messageId);

                if (messageDto.PublicMessageId == new Guid())
                {
                    return ReturnApiResponse.Failure(response, "Message Id not found.");
                }

                return ReturnApiResponse.Success(response, messageDto);
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.GetByIdAsync", ex);
                return ReturnApiResponse.Failure(response, "Error getting message.");
            }
        }

        public async Task<ApiResponse<PublicGroupMessageDto>> AddAsync(PublicGroupMessageDto messageDto)
        {
            ApiResponse<PublicGroupMessageDto> response = new();

            (bool, string) messageChecks = NewMessageChecks(messageDto);
            if (messageChecks.Item1 == false)
            {
                return ReturnApiResponse.Failure(response, messageChecks.Item2);
            }

            try
            {
                PublicGroupMessages newMessage = NewPublicMessage(messageDto);
                bool isSuccess = await _publicMessageDataAccess.AddAsync(newMessage);

                if (!isSuccess)
                {
                    return ReturnApiResponse.Failure(response, "Error saving new message.");
                }

                PublicGroupMessageDto newDto = NewModelToDto(newMessage, messageDto);

                return ReturnApiResponse.Success(response, newDto);
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.AddAsync", ex);
                return ReturnApiResponse.Failure(response, "Error saving new message.");
            }
        }

        public async Task<ApiResponse<PublicGroupMessageDto>> ModifyAsync(ModifyPublicGroupMessageDto dtoToModify)
        {
            ApiResponse<PublicGroupMessageDto> response = new();

            try
            {
                // Check that message exists before proceeding to modify
                if (await _publicMessageDataAccess.Exists(dtoToModify.PublicMessageId) == false)
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, "Message Id not found.");
                }

                PublicGroupMessages messageToModify = await _publicMessageDataAccess.GetByMessageIdAsync(dtoToModify.PublicMessageId);
                messageToModify = ModifyDtoToModel(dtoToModify, messageToModify);

                bool isSuccess = await _publicMessageDataAccess.ModifyAsync(messageToModify);

                if (!isSuccess)
                {
                    return ReturnApiResponse.Failure(response, "Error modifying message.");
                }

                PublicGroupMessageDto returnDto = ModifiedModelToDto(messageToModify);

                return ReturnApiResponse.Success(response, returnDto);
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.ModifyAsync", ex);
                return ReturnApiResponse.Failure(response, "Error modifying message.");
            }
        }

        // Check that message exists. If true, then delete and messages that are a reponse to this message (ReponseMessageId).
        // Finally delete message
        public async Task<ApiResponse<PublicGroupMessageDto>> DeleteAsync(Guid messageId)
        {
            ApiResponse<PublicGroupMessageDto> response = new();

            try
            {
                // Check message exists
                if (!await _publicMessageDataAccess.Exists(messageId))
                {
                    return ReturnApiResponse.Failure(response, "Message Id not found.");
                }

                // Find message to delete
                PublicGroupMessages messageToDelete = await _publicMessageDataAccess.GetByMessageIdAsync(messageId);

                // Delete all messages that are a response to this message
                bool responseMessagesDeleted = await _publicMessageDataAccess.DeleteMessagesByResponseMessageIdAsync(messageId);
                if (!responseMessagesDeleted)
                {
                    return ReturnApiResponse.Failure(response, "Response messages not deleted.");
                }

                // Delete the message
                bool isSuccess = await _publicMessageDataAccess.DeleteAsync(messageToDelete);
                if (!isSuccess)
                {
                    return ReturnApiResponse.Failure(response, "Error deleting message.");
                }

                return ReturnApiResponse.Success(response, ModelToDto(messageToDelete));
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.DeleteAsync", ex);
                return ReturnApiResponse.Failure(response, "Error deleting message.");
            }
        }

        public async Task<bool> DeleteAllMessagesInGroupAsync(int chatGroupId)
        {
            return await _publicMessageDataAccess.DeleteAllMessagesInGroupAsync(chatGroupId);
        }

        #region PRIVATE METHODS

        private (bool, string) NewMessageChecks(PublicGroupMessageDto messageDto)
        {
            bool passesChecks = true;
            string errorMessage = "";

            if (Guid.Parse(messageDto.UserId) == new Guid())
            {
                passesChecks = false;
                errorMessage += "[Invalid UserId]";
            }
            if (messageDto.Text.IsNullOrEmpty())
            {
                passesChecks = false;
                errorMessage += "[Text is empty or null]";
            }

            return (passesChecks, errorMessage);
        }

        private PublicGroupMessageDto ModelToDto(PublicGroupMessages message)
        {
            return new()
            {
                PublicMessageId = message.PublicMessageId,
                UserId          = message.UserId,
                ChatGroupId     = message.ChatGroupId,
                Text            = message.Text,
                MessageDateTime = message.MessageDateTime,
                ReplyMessageId  = message.ReplyMessageId,
                PictureLink     = message.PictureLink
            };
        }

        private PublicGroupMessages NewPublicMessage(PublicGroupMessageDto messageDto)
        {
            return new()
            {
                PublicMessageId = Guid.NewGuid(),
                UserId          = messageDto.UserId.ToString(),
                ChatGroupId     = messageDto.ChatGroupId,
                Text            = messageDto.Text,
                MessageDateTime = DateTime.Now,
                ReplyMessageId  = messageDto.ReplyMessageId,
                PictureLink     = messageDto.PictureLink
            };
        }

        // Map PublicMessage fields to return dto object. This object should retain dto specific fields
        private PublicGroupMessageDto NewModelToDto(PublicGroupMessages newMessage, PublicGroupMessageDto dtoMessage)
        {
            return new()
            {
                PublicMessageId = newMessage.PublicMessageId,
                UserId          = newMessage.UserId,
                UserName        = dtoMessage.UserName,
                ChatGroupId     = newMessage.ChatGroupId,
                ChatGroupName   = dtoMessage.ChatGroupName,
                Text            = newMessage.Text,
                MessageDateTime = newMessage.MessageDateTime,
                ReplyMessageId  = newMessage.ReplyMessageId,
                PictureLink     = newMessage.PictureLink
            };
        }

        // Return PublicMessage with existing Id fields. Id fields should not be updated when modifying a message.
        private PublicGroupMessages ModifyDtoToModel(ModifyPublicGroupMessageDto dtoMessage, PublicGroupMessages message)
        {
            return new()
            {
                PublicMessageId = message.PublicMessageId,
                UserId          = message.UserId,
                ChatGroupId     = message.ChatGroupId,
                Text            = dtoMessage.Text,
                MessageDateTime = message.MessageDateTime,
                ReplyMessageId  = dtoMessage.ReplyMessageId,
                PictureLink     = dtoMessage.PictureLink
            };
        }

        // after modified public message is saved, convert back to a dto
        private PublicGroupMessageDto ModifiedModelToDto(PublicGroupMessages modifiedMessage)
        {
            return new()
            {
                PublicMessageId = modifiedMessage.PublicMessageId,
                UserId          = modifiedMessage.PublicMessageId.ToString(),
                ChatGroupId     = modifiedMessage.ChatGroupId,
                MessageDateTime = modifiedMessage.MessageDateTime,
                Text            = modifiedMessage.Text,
                ReplyMessageId  = modifiedMessage.ReplyMessageId, 
                PictureLink     = modifiedMessage.PictureLink
            };
        }

        #endregion
    }
}
