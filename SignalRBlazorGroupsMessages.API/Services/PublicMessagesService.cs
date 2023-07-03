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

        public async Task<ListPublicMessagesDto> GetMessagesByGroupIdAsync(int groupId, int currentItemsCount)
        {
            List<PublicMessages> listPublicMessages =  await _dataAccess.GetMessagesByGroupIdAsync(groupId, currentItemsCount);

            ListPublicMessagesDto listPublicMessagesDto = new()
            {
                PublicMessages = listPublicMessages,
                CurrentItemsCount = currentItemsCount
            };

            return listPublicMessagesDto;
        }

        public async Task<ListPublicMessagesDto> GetMessagesByUserIdAsync(string userId, int currentItemsCount)
        {
            if (userId.IsNullOrEmpty())
            {
                ListPublicMessagesDto emptyListPublicMessagesDto = new()
                {
                    CurrentItemsCount = currentItemsCount
                };

                return emptyListPublicMessagesDto;
            }
            
            List<PublicMessages> listPublicMessagesFromUser = await _dataAccess.GetMessagesByUserIdAsync(userId, currentItemsCount);

            ListPublicMessagesDto listPublicMessagesDto = new()
            {
                PublicMessages = listPublicMessagesFromUser,
                CurrentItemsCount = currentItemsCount
            };

            return listPublicMessagesDto;
        }

    }
}
