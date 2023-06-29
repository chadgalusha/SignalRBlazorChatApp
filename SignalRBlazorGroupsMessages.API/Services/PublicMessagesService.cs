using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;

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


    }
}
