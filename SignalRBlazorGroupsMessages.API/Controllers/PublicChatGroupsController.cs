using Microsoft.AspNetCore.Mvc;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorGroupsMessages.API.Controllers
{
    [Route("api/[controller]/")]
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

        [HttpGet("byid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<PublicChatGroupsDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PublicChatGroupsDto>>> GetByIdAsync(
            [FromQuery] int groupId)
        {
            if (groupId < 0)
                return BadRequest("Invalid data: " + nameof(groupId));

            ApiResponse<PublicChatGroupsDto> dtoResponse = await _service.GetByIdAsync(groupId);
            _serilogger.GetRequest(GetIpv4Address(), dtoResponse);

            return Ok(dtoResponse);
        }

        #region PRIVATE METHODS

        private string GetIpv4Address()
        {
            return HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
        }

        #endregion
    }
}
