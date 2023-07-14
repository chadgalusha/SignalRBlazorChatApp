using Microsoft.AspNetCore.Mvc;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorGroupsMessages.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicChatGroupsController : ControllerBase
    {
        private readonly IPublicChatGroupsService _service;
        private readonly ISerilogger _serilogger;

        public PublicChatGroupsController(IPublicChatGroupsService service, ISerilogger serilogger)
        {
            _service = service ?? throw new Exception(nameof(service));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PublicChatGroupsDto>>>> GetPublicChatGroupsAsync()
        {
            ApiResponse<List<PublicChatGroupsDto>> dtoList = await _service.GetListPublicChatGroupsAsync();
            _serilogger.GetRequest(GetIpv4Address(), dtoList);

            return dtoList;
        }

        #region PRIVATE METHODS

        private string GetIpv4Address()
        {
            return HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
        }

        #endregion
    }
}
