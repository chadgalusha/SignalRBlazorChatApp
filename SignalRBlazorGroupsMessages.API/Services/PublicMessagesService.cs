using ChatApplicationModels;
using Microsoft.IdentityModel.Tokens;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public class PublicMessagesService
    {
        private readonly IPublicMessagesDataAccess _dataAccess;
        private readonly ISerilogger _serilogger;

        public PublicMessagesService(IPublicMessagesDataAccess dataAccess, ISerilogger serilogger)
        {
            _dataAccess = dataAccess;
            _serilogger = serilogger;
        }

        public async Task<ApiResponse<ListPublicMessagesDto>> GetMessagesByGroupIdAsync(int groupId, int numberItemsToSkip)
        {
            ApiResponse<ListPublicMessagesDto> response = new();
            ListPublicMessagesDto listPublicMessagesDto = new();

            try
            {
                List<PublicMessagesView> listPublicMessagesView = await _dataAccess.GetMessagesByGroupIdAsync(groupId, numberItemsToSkip);

                listPublicMessagesDto.PublicMessages = ViewToDto(listPublicMessagesView);
                listPublicMessagesDto.CurrentItemsCount = numberItemsToSkip;

                response = ReturnApiResponse.Success(response, listPublicMessagesDto);

                return response;
            }
            catch (Exception ex)
            {
                _serilogger.LogPublicMessageError("PublicMessagesService.GetMessagesByGroupIdAsync", ex);

                response = ReturnApiResponse.Failure(response, "Error getting messages.");
                response.Data = null;

                return response;
            }
        }

        public async Task<ApiResponse<ListPublicMessagesDto>> GetMessagesByUserIdAsync(Guid userId, int numberItemsToSkip)
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
                List<PublicMessagesView> listPublicMessagesView = await _dataAccess.GetMessagesByUserIdAsync(userId, numberItemsToSkip);

                listPublicMessagesDto.PublicMessages = ViewToDto(listPublicMessagesView);
                listPublicMessagesDto.CurrentItemsCount = numberItemsToSkip;

                response = ReturnApiResponse.Success(response, listPublicMessagesDto);

                return response;
            }
            catch (Exception ex)
            {
                _serilogger.LogPublicMessageError("PublicMessagesService.GetMessagesByUserIdAsync", ex);

                response = ReturnApiResponse.Failure(response, "Error getting messages.");
                response.Data = null;

                return response;
            }
        }

        public async Task<ApiResponse<PublicMessageDto>> GetPublicMessageByIdAsync(string messageId)
        {
            return new();
        }
        #region PRIVATE METHODS

        private List<PublicMessageDto> ViewToDto(List<PublicMessagesView> publicMessagesViewList)
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

        #endregion
    }
}
