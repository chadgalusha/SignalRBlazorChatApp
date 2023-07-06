using ChatApplicationModels;
using Microsoft.IdentityModel.Tokens;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public class PublicMessagesService
    {
        private readonly IPublicMessagesDataAccess _publicMessageDataAccess;
        private readonly ISerilogger _serilogger;

        public PublicMessagesService(IPublicMessagesDataAccess publicMessageDataAccess, ISerilogger serilogger)
        {
            _publicMessageDataAccess = publicMessageDataAccess ?? throw new Exception(nameof(publicMessageDataAccess));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        public async Task<ApiResponse<ListPublicMessagesDto>> GetByGroupIdAsync(int groupId, int numberItemsToSkip)
        {
            ApiResponse<ListPublicMessagesDto> response = new();
            ListPublicMessagesDto listPublicMessagesDto = new();

            try
            {
                List<PublicMessagesView> listPublicMessagesView = await _publicMessageDataAccess.GetViewListByGroupIdAsync(groupId, numberItemsToSkip);

                listPublicMessagesDto.PublicMessages = ViewListToDtoList(listPublicMessagesView);
                listPublicMessagesDto.CurrentItemsCount = numberItemsToSkip;

                response = ReturnApiResponse.Success(response, listPublicMessagesDto);

                return response;
            }
            catch (Exception ex)
            {
                _serilogger.LogPublicMessageError("PublicMessagesService.GetByGroupIdAsync", ex);

                response = ReturnApiResponse.Failure(response, "Error getting messages.");
                response.Data = null;

                return response;
            }
        }

        public async Task<ApiResponse<ListPublicMessagesDto>> GetByUserIdAsync(Guid userId, int numberItemsToSkip)
        {
            ListPublicMessagesDto listPublicMessagesDto = new();
            ApiResponse<ListPublicMessagesDto> response = new();

            if (userId.ToString().IsNullOrEmpty())
            {
                response.Data = null;
                response = ReturnApiResponse.Failure(response, "User Id is not valid.");

                return response;
            }

            try
            {
                List<PublicMessagesView> listPublicMessagesView = await _publicMessageDataAccess.GetViewListByUserIdAsync(userId, numberItemsToSkip);

                listPublicMessagesDto.PublicMessages = ViewListToDtoList(listPublicMessagesView);
                listPublicMessagesDto.CurrentItemsCount = numberItemsToSkip;

                response = ReturnApiResponse.Success(response, listPublicMessagesDto);

                return response;
            }
            catch (Exception ex)
            {
                _serilogger.LogPublicMessageError("PublicMessagesService.GetByUserIdAsync", ex);

                response = ReturnApiResponse.Failure(response, "Error getting messages.");
                response.Data = null;

                return response;
            }
        }

        public async Task<ApiResponse<PublicMessageDto>> GetByMessageIdAsync(Guid messageId)
        {
            ApiResponse<PublicMessageDto> response = new();

            if (messageId.ToString().IsNullOrEmpty())
            {
                response.Data = null;
                response = ReturnApiResponse.Failure(response, "Message Id is not valid.");

                return response;
            }

            try
            {
                PublicMessagesView messageView = await _publicMessageDataAccess.GetViewByMessageIdAsync(messageId);
                PublicMessageDto messageDto = ViewToDto(messageView);

                response = ReturnApiResponse.Success(response, messageDto);

                return response;
            }
            catch (Exception ex)
            {
                _serilogger.LogPublicMessageError("PublicMessagesService.GetByIdAsync", ex);

                response = ReturnApiResponse.Failure(response, "Error getting message.");
                response.Data = null;

                return response;
            }
        }

        public async Task<ApiResponse<PublicMessageDto>> AddAsync(PublicMessageDto messageDto)
        {
            ApiResponse<PublicMessageDto> response = new();

            (bool, string) messageChecks = NewMessageChecks(messageDto);
            if (messageChecks.Item1 == false)
            {
                response.Data = null;
                response.Message = messageChecks.Item2;

                return response;
            }

            try
            {
                PublicMessages newMessage = NewPublicMessage(messageDto);
                bool isSuccess = await _publicMessageDataAccess.AddAsync(newMessage);

                if (!isSuccess)
                {
                    response = ReturnApiResponse.Failure(response, "Error saving new message.");
                    response.Data = null;

                    return response;
                }

                PublicMessageDto newDto = PublicMessageToPublicMessageDto(newMessage, messageDto);
                response = ReturnApiResponse.Success(response, newDto);

                return response;
            }
            catch (Exception ex)
            {
                _serilogger.LogPublicMessageError("PublicMessagesService.AddAsync", ex);

                response = ReturnApiResponse.Failure(response, "Error saving new message.");
                response.Data = null;

                return response;
            }
        }

        public async Task<ApiResponse<PublicMessageDto>> ModifyAsync(PublicMessageDto dtoMessage)
        {
            ApiResponse<PublicMessageDto> response = new();

            if (await _publicMessageDataAccess.Exists(dtoMessage.PublicMessageId) == false)
            {
                response = ReturnApiResponse.Failure(response, "Message Id not found.");
                response.Data = null;

                return response;
            }

            try
            {
                PublicMessages messageToModify = await _publicMessageDataAccess.GetByMessageIdAsync(dtoMessage.PublicMessageId);
                messageToModify = DtoToPublicMessage(dtoMessage, messageToModify);

                bool isSuccess = await _publicMessageDataAccess.ModifyAsync(messageToModify);

                if (!isSuccess)
                {
                    response = ReturnApiResponse.Failure(response, "Message modification not saved.");
                    response.Data = null;

                    return response;
                }

                PublicMessageDto returnDto = PublicMessageToPublicMessageDto(messageToModify, dtoMessage);
                response = ReturnApiResponse.Success(response, returnDto);

                return response;
            }
            catch (Exception ex)
            {
                _serilogger.LogPublicMessageError("PublicMessagesService.ModifyAsync", ex);

                response = ReturnApiResponse.Failure(response, "Error modifying message.");
                response.Data = null;

                return response;
            }
        }

        // Check that message exists. If true, then delete and messages that are a reponse to this message (ReponseMessageId).
        // Finally delete message
        public async Task<ApiResponse<PublicMessageDto>> DeleteAsync(PublicMessageDto dtoMessage)
        {
            ApiResponse<PublicMessageDto> response = new();

            if (await _publicMessageDataAccess.Exists(dtoMessage.PublicMessageId) == false)
            {
                response = ReturnApiResponse.Failure(response, "Message Id not found.");
                response.Data = null;

                return response;
            }

            try
            {
                PublicMessages messageToDelete = await _publicMessageDataAccess.GetByMessageIdAsync(dtoMessage.PublicMessageId);
                messageToDelete = DtoToPublicMessage(dtoMessage, messageToDelete);

                bool resultMessageDelete = await _publicMessageDataAccess.DeleteMessagesByResponseMessageIdAsync(messageToDelete.PublicMessageId);
                if (!resultMessageDelete)
                {
                    response = ReturnApiResponse.Failure(response, "Response messages not deleted.");
                    response.Data = null;

                    return response;
                }

                bool isSuccess = await _publicMessageDataAccess.DeleteAsync(messageToDelete);
                if (!isSuccess)
                {
                    response = ReturnApiResponse.Failure(response, "Message not deleted.");
                    response.Data = null;

                    return response;
                }

                PublicMessageDto returnDto = PublicMessageToPublicMessageDto(messageToDelete, dtoMessage);
                response = ReturnApiResponse.Success(response, returnDto);

                return response;
            }
            catch (Exception ex)
            {
                _serilogger.LogPublicMessageError("PublicMessagesService.DeleteAsync", ex);

                response = ReturnApiResponse.Failure(response, "Error deleting message.");
                response.Data = null;

                return response;
            }
        }

        #region PRIVATE METHODS

        private PublicMessageDto ViewToDto(PublicMessagesView publicMessagesView)
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

        private List<PublicMessageDto> ViewListToDtoList(List<PublicMessagesView> publicMessagesViewList)
        {
            List<PublicMessageDto> dtoList = new();

            foreach (var viewItem in publicMessagesViewList)
            {
                PublicMessageDto dto = new()
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

        private (bool, string) NewMessageChecks(PublicMessageDto messageDto)
        {
            bool passesChecks = true;
            string errorMessage = "";

            if (messageDto.UserId == new Guid())
                passesChecks = false;
                errorMessage += "[Invalid UserId]";
            if (messageDto.Text.IsNullOrEmpty())
                passesChecks = false;
                errorMessage += "[Text is empty or null]";

            return (passesChecks, errorMessage);
        }

        private PublicMessages NewPublicMessage(PublicMessageDto messageDto)
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
        private PublicMessageDto PublicMessageToPublicMessageDto(PublicMessages newMessage, PublicMessageDto dtoMessage)
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
        private PublicMessages DtoToPublicMessage(PublicMessageDto dtoMessage, PublicMessages message)
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
