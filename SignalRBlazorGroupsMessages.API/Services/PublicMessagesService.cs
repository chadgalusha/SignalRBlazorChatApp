using ChatApplicationModels;
using Microsoft.IdentityModel.Tokens;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public class PublicMessagesService : IPublicMessagesService
    {
        private readonly IPublicMessagesDataAccess _publicMessageDataAccess;
        private readonly ISerilogger _serilogger;

        public PublicMessagesService(IPublicMessagesDataAccess publicMessageDataAccess, ISerilogger serilogger)
        {
            _publicMessageDataAccess = publicMessageDataAccess ?? throw new Exception(nameof(publicMessageDataAccess));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        public async Task<ApiResponse<List<PublicGroupMessageDto>>> GetListByGroupIdAsync(int groupId, int numberItemsToSkip)
        {
            ApiResponse<List<PublicGroupMessageDto>> response = new();

            try
            {
                List<PublicGroupMessagesView> viewList = await _publicMessageDataAccess.GetViewListByGroupIdAsync(groupId, numberItemsToSkip);
                List<PublicGroupMessageDto> dtoList = ViewListToDtoList(viewList);

                response = ReturnApiResponse.Success(response, dtoList);

                return response;
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.GetByGroupIdAsync", ex);

                response.Data = null;
                return ReturnApiResponse.Failure(response, "Error getting messages.");
            }
        }

        public async Task<ApiResponse<List<PublicGroupMessageDto>>> GetViewListByUserIdAsync(Guid userId, int numberItemsToSkip)
        {
            ApiResponse<List<PublicGroupMessageDto>> response = new();

            try
            {
                List<PublicGroupMessagesView> listPublicMessagesView = await _publicMessageDataAccess.GetViewListByUserIdAsync(userId, numberItemsToSkip);
                List<PublicGroupMessageDto> listDto = ViewListToDtoList(listPublicMessagesView);

                response = ReturnApiResponse.Success(response, listDto);

                return response;
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.GetByUserIdAsync", ex);

                response.Data = null;
                return ReturnApiResponse.Failure(response, "Error getting messages.");
            }
        }

        public async Task<ApiResponse<PublicGroupMessageDto>> GetByMessageIdAsync(Guid messageId)
        {
            ApiResponse<PublicGroupMessageDto> response = new();

            try
            {
                PublicGroupMessagesView messageView = await _publicMessageDataAccess.GetViewByMessageIdAsync(messageId);

                if (messageView.PublicMessageId == new Guid())
                {
                    return ReturnApiResponse.Failure(response, "Message Id not found.");
                }

                PublicGroupMessageDto messageDto = ViewToDto(messageView);

                response = ReturnApiResponse.Success(response, messageDto);

                return response;
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.GetByIdAsync", ex);

                response.Data = null;
                return ReturnApiResponse.Failure(response, "Error getting message.");
            }
        }

        public async Task<ApiResponse<PublicGroupMessageDto>> AddAsync(PublicGroupMessageDto messageDto)
        {
            ApiResponse<PublicGroupMessageDto> response = new();

            (bool, string) messageChecks = NewMessageChecks(messageDto);
            if (messageChecks.Item1 == false)
            {
                response.Success = false;
                response.Message = messageChecks.Item2;
                response.Data = null;

                return response;
            }

            try
            {
                PublicGroupMessages newMessage = NewPublicMessage(messageDto);
                bool isSuccess = await _publicMessageDataAccess.AddAsync(newMessage);

                if (!isSuccess)
                {
                    response = ReturnApiResponse.Failure(response, "Error saving new message.");
                    response.Data = null;

                    return response;
                }

                PublicGroupMessageDto newDto = PublicMessageToPublicMessageDto(newMessage, messageDto);
                response = ReturnApiResponse.Success(response, newDto);

                return response;
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.AddAsync", ex);

                response.Data = null;
                return ReturnApiResponse.Failure(response, "Error saving new message.");
            }
        }

        public async Task<ApiResponse<PublicGroupMessageDto>> ModifyAsync(PublicGroupMessageDto dtoMessage)
        {
            ApiResponse<PublicGroupMessageDto> response = new();

            try
            {
                // Check that message exists before proceeding to modify
                if (await _publicMessageDataAccess.Exists(dtoMessage.PublicMessageId) == false)
                {
                    response = ReturnApiResponse.Failure(response, "Message Id not found.");
                    response.Data = null;

                    return response;
                }

                PublicGroupMessages messageToModify = await _publicMessageDataAccess.GetByMessageIdAsync(dtoMessage.PublicMessageId);
                messageToModify = DtoToPublicMessage(dtoMessage, messageToModify);

                bool isSuccess = await _publicMessageDataAccess.ModifyAsync(messageToModify);

                if (!isSuccess)
                {
                    response = ReturnApiResponse.Failure(response, "Message modification not saved.");
                    response.Data = null;

                    return response;
                }

                PublicGroupMessageDto returnDto = PublicMessageToPublicMessageDto(messageToModify, dtoMessage);
                response = ReturnApiResponse.Success(response, returnDto);

                return response;
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.ModifyAsync", ex);

                response.Data = null;
                return ReturnApiResponse.Failure(response, "Error modifying message.");
            }
        }

        // Check that message exists. If true, then delete and messages that are a reponse to this message (ReponseMessageId).
        // Finally delete message
        public async Task<ApiResponse<PublicGroupMessageDto>> DeleteAsync(PublicGroupMessageDto dtoMessage)
        {
            ApiResponse<PublicGroupMessageDto> response = new();

            try
            {
                // Check message exists
                if (!await _publicMessageDataAccess.Exists(dtoMessage.PublicMessageId))
                {
                    response = ReturnApiResponse.Failure(response, "Message Id not found.");
                    response.Data = null;

                    return response;
                }

                // Find message to delete
                PublicGroupMessages messageToDelete = await _publicMessageDataAccess.GetByMessageIdAsync(dtoMessage.PublicMessageId);
                messageToDelete = DtoToPublicMessage(dtoMessage, messageToDelete);

                // Delete all messages that are a response to this message
                bool resultMessageDelete = await _publicMessageDataAccess.DeleteMessagesByResponseMessageIdAsync(messageToDelete.PublicMessageId);
                if (!resultMessageDelete)
                {
                    response = ReturnApiResponse.Failure(response, "Response messages not deleted.");
                    response.Data = null;

                    return response;
                }

                // Delete the message
                bool isSuccess = await _publicMessageDataAccess.DeleteAsync(messageToDelete);
                if (!isSuccess)
                {
                    response = ReturnApiResponse.Failure(response, "Message not deleted.");
                    response.Data = null;

                    return response;
                }

                return ReturnApiResponse.Success(response, dtoMessage);
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.DeleteAsync", ex);

                response.Data = null;
                return ReturnApiResponse.Failure(response, "Error deleting message.");
            }
        }

        public async Task<bool> DeleteAllMessagesInGroupAsync(int chatGroupId)
        {
            return await _publicMessageDataAccess.DeleteAllMessagesInGroupAsync(chatGroupId);
        }

        #region PRIVATE METHODS

        private PublicGroupMessageDto ViewToDto(PublicGroupMessagesView publicMessagesView)
        {
            return new()
            {
                PublicMessageId = publicMessagesView.PublicMessageId,
                UserId          = publicMessagesView.UserId,
                UserName        = publicMessagesView.UserName,
                ChatGroupId     = publicMessagesView.ChatGroupId,
                ChatGroupName   = publicMessagesView.ChatGroupName,
                Text            = publicMessagesView.Text,
                MessageDateTime = publicMessagesView.MessageDateTime,
                ReplyMessageId  = publicMessagesView.ReplyMessageId,
                PictureLink     = publicMessagesView.PictureLink
            };
        }

        private List<PublicGroupMessageDto> ViewListToDtoList(List<PublicGroupMessagesView> publicMessagesViewList)
        {
            List<PublicGroupMessageDto> dtoList = new();

            foreach (var viewItem in publicMessagesViewList)
            {
                PublicGroupMessageDto dto = new()
                {
                    PublicMessageId = viewItem.PublicMessageId,
                    UserId          = viewItem.UserId,
                    UserName        = viewItem.UserName,
                    ChatGroupId     = viewItem.ChatGroupId,
                    ChatGroupName   = viewItem.ChatGroupName,
                    Text            = viewItem.Text,
                    MessageDateTime = viewItem.MessageDateTime,
                    ReplyMessageId  = viewItem.ReplyMessageId,
                    PictureLink     = viewItem.PictureLink
                };
                dtoList.Add(dto);
            }

            return dtoList;
        }

        private (bool, string) NewMessageChecks(PublicGroupMessageDto messageDto)
        {
            bool passesChecks = true;
            string errorMessage = "";

            if (messageDto.UserId == new Guid())
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

        private PublicGroupMessages NewPublicMessage(PublicGroupMessageDto messageDto)
        {
            return new()
            {
                PublicMessageId = Guid.NewGuid(),
                UserId          = messageDto.UserId,
                ChatGroupId     = messageDto.ChatGroupId,
                Text            = messageDto.Text,
                MessageDateTime = messageDto.MessageDateTime,
                ReplyMessageId  = messageDto.ReplyMessageId,
                PictureLink     = messageDto.PictureLink
            };
        }

        // Map PublicMessage fields to return dto object. This object should retain dto specific fields
        private PublicGroupMessageDto PublicMessageToPublicMessageDto(PublicGroupMessages newMessage, PublicGroupMessageDto dtoMessage)
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
        private PublicGroupMessages DtoToPublicMessage(PublicGroupMessageDto dtoMessage, PublicGroupMessages message)
        {
            return new()
            {
                PublicMessageId = message.PublicMessageId,
                UserId          = message.UserId,
                ChatGroupId     = message.ChatGroupId,
                Text            = dtoMessage.Text,
                MessageDateTime = dtoMessage.MessageDateTime,
                ReplyMessageId  = dtoMessage.ReplyMessageId,
                PictureLink     = dtoMessage.PictureLink
            };
        }

        #endregion
    }
}
