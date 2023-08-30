using ChatApplicationModels;
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
                return ReturnApiResponse.Failure(response, ErrorMessages.RetrievingItems);
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
                return ReturnApiResponse.Failure(response, ErrorMessages.RetrievingItems);
            }
        }

        public async Task<ApiResponse<PublicGroupMessageDto>> GetByMessageIdAsync(Guid messageId)
        {
            ApiResponse<PublicGroupMessageDto> apiResponse = new();

            try
            {
                PublicGroupMessageDto messageDto = await _publicMessageDataAccess.GetDtoByMessageIdAsync(messageId);

                if (messageDto.PublicMessageId == new Guid())
                {
                    return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RecordNotFound);
                }

                return ReturnApiResponse.Success(apiResponse, messageDto);
            }
            catch (InvalidOperationException ex)
            {
                _serilogger.ChatGroupError("PublicMessagesService.GetByMessageIdAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RecordNotFound);
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.GetByMessageIdAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RetrievingItems);
            }
        }

        public async Task<ApiResponse<PublicGroupMessageDto>> AddAsync(CreatePublicGroupMessageDto createDto)
        {
            ApiResponse<PublicGroupMessageDto> apiResponse = new();

            try
            {
                PublicGroupMessages newMessage = CreateDtoToModel(createDto);

                return await _publicMessageDataAccess.AddAsync(newMessage) ?
                    ReturnApiResponse.Success(apiResponse, await _publicMessageDataAccess.GetDtoByMessageIdAsync(newMessage.PublicMessageId)) :
                    ReturnApiResponse.Failure(apiResponse, ErrorMessages.AddingItem);
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.AddAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.AddingItem);
            }
        }

        public async Task<ApiResponse<PublicGroupMessageDto>> ModifyAsync(ModifyPublicGroupMessageDto dtoToModify, string jwtUserId)
        {
            ApiResponse<PublicGroupMessageDto> apiResponse = new();

            try
            {
                PublicGroupMessages messageToModify = await _publicMessageDataAccess.GetByMessageIdAsync(dtoToModify.PublicMessageId);

                (bool, string) modifyMessageChecks = ModifyMessageChecks(dtoToModify, messageToModify, jwtUserId);
                if (!modifyMessageChecks.Item1)
                {
                    return ReturnApiResponse.Failure(apiResponse, modifyMessageChecks.Item2);
                }

                messageToModify = ModifyDtoToModel(dtoToModify, messageToModify);

                return await _publicMessageDataAccess.ModifyAsync(messageToModify) ?
                    ReturnApiResponse.Success(apiResponse, await _publicMessageDataAccess.GetDtoByMessageIdAsync(messageToModify.PublicMessageId)) :
                    ReturnApiResponse.Failure(apiResponse, ErrorMessages.ModifyingItem);
            }
            catch (InvalidOperationException ex)
            {
                _serilogger.ChatGroupError("PublicMessagesService.ModifyAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RecordNotFound);
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.ModifyAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.ModifyingItem);
            }
        }

        // Check that message exists. If true, then delete and messages that are a reponse to this message (ReponseMessageId).
        // Finally delete message
        public async Task<ApiResponse<PublicGroupMessageDto>> DeleteAsync(Guid messageId, string jwtUserId)
        {
            ApiResponse<PublicGroupMessageDto> apiResponse = new();

            try
            {
                PublicGroupMessages messageToDelete = await _publicMessageDataAccess.GetByMessageIdAsync(messageId);

                // Delete all messages that are a response to this message
                (bool, string) deleteMessageChecks = await DeleteMessageChecks(messageToDelete, jwtUserId);
                if (!deleteMessageChecks.Item1)
                {
                    return ReturnApiResponse.Failure(apiResponse, deleteMessageChecks.Item2);
                }

                return await _publicMessageDataAccess.DeleteAsync(messageToDelete) ?
                    ReturnApiResponse.Success(apiResponse, new()) :
                    ReturnApiResponse.Failure(apiResponse, ErrorMessages.DeletingItem);
            }
            catch (InvalidOperationException ex)
            {
                _serilogger.ChatGroupError("PublicMessagesService.DeleteAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.RecordNotFound);
            }
            catch (Exception ex)
            {
                _serilogger.PublicMessageError("PublicMessagesService.DeleteAsync", ex);
                return ReturnApiResponse.Failure(apiResponse, ErrorMessages.DeletingItem);
            }
        }

        public async Task<bool> DeleteAllMessagesInGroupAsync(int chatGroupId)
        {
            return await _publicMessageDataAccess.DeleteAllMessagesInGroupAsync(chatGroupId);
        }

        #region PRIVATE METHODS

        private (bool, string) ModifyMessageChecks(ModifyPublicGroupMessageDto modifyDto, PublicGroupMessages message, string jwtUserId) 
        {
            if (message.UserId != jwtUserId)
                return (false, ErrorMessages.InvalidUserId);
            if (modifyDto.Text == message.Text 
                && modifyDto.ReplyMessageId == message.ReplyMessageId 
                && modifyDto.PictureLink == message.PictureLink)
                return (false, ErrorMessages.NoModification);
            return (true, "");
        }

        private async Task<(bool, string)> DeleteMessageChecks(PublicGroupMessages message, string jwtUserId)
        {
            if (message.UserId != jwtUserId)
                return (false, ErrorMessages.InvalidUserId);
            if (!await _publicMessageDataAccess.DeleteMessagesByResponseMessageIdAsync(message.PublicMessageId))
                return (false, ErrorMessages.DeletingMessages);
            return (true, "");
        }

        private PublicGroupMessages CreateDtoToModel(CreatePublicGroupMessageDto createDto)
        {
            return new()
            {
                PublicMessageId = Guid.NewGuid(),
                UserId          = createDto.UserId,
                ChatGroupId     = createDto.ChatGroupId,
                Text            = createDto.Text,
                MessageDateTime = DateTime.Now,
                ReplyMessageId  = createDto.ReplyMessageId,
                PictureLink     = createDto.PictureLink
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

        #endregion
    }
}
