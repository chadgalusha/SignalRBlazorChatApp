using Microsoft.AspNetCore.Mvc;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorGroupsMessages.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class PublicMessagesController : ControllerBase
    {
        private readonly IPublicMessagesService _service;
        private readonly ISerilogger _serilogger;

        public PublicMessagesController(IPublicMessagesService service, ISerilogger serilogger)
        {
            _service    = service ?? throw new Exception(nameof(service));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        // GET: api/<PublicMessagesController>/bygroupid
        [HttpGet("bygroupid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<PublicGroupMessageDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<PublicGroupMessageDto>>>> GetListByGroupIdAsync(
            [FromQuery] int groupId, int numberItemsToSkip)
        {
            if (groupId < 0)
                return BadRequest("Invalid data: " + nameof(groupId));
            if (numberItemsToSkip < 0)
                return BadRequest("Invalid data: "+nameof(numberItemsToSkip));

            ApiResponse<List<PublicGroupMessageDto>> dtoList = await _service.GetListByGroupIdAsync(groupId, numberItemsToSkip);
            _serilogger.GetRequest(GetIpv4Address(), dtoList);

            return Ok(dtoList);
        }

        // GET: api/<PublicMessagesController>/byuserid
        [HttpGet("byuserid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<PublicGroupMessageDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<PublicGroupMessageDto>>>> GetListByUserIdAsync(
            [FromQuery] Guid userId, int numberItemsToSkip)
        {
            if (userId == new Guid())
                return BadRequest("Invalid data: " + nameof(userId));
            if (numberItemsToSkip < 0)
                return BadRequest("Invalid data: " + nameof(numberItemsToSkip));

            ApiResponse<List<PublicGroupMessageDto>> listDtoResponse = await _service.GetViewListByUserIdAsync(userId, numberItemsToSkip);
            _serilogger.GetRequest(GetIpv4Address(), listDtoResponse);

            return Ok(listDtoResponse);
        }

        // GET api/<PublicMessagesController>/bymessageid
        [HttpGet("bymessageid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PublicGroupMessageDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PublicGroupMessageDto>>> GetByMessageIdAsync(
            [FromQuery] Guid messageId)
        {
            if (messageId == new Guid())
                return BadRequest("Invalid data: " + nameof(messageId));

            ApiResponse<PublicGroupMessageDto> dtoResponse = await _service.GetByMessageIdAsync(messageId);
            _serilogger.GetRequest(GetIpv4Address(), dtoResponse);

            return Ok(dtoResponse);
        }

        // POST api/<PublicMessagesController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PublicGroupMessageDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PublicGroupMessageDto>>> Post([FromBody] PublicGroupMessageDto dtoToCreate)
        {
            if (!ModelState.IsValid || dtoToCreate == null)
            {
                return BadRequest(ModelState);
            }

            ApiResponse<PublicGroupMessageDto> dtoResponse = await _service.AddAsync(dtoToCreate);
            _serilogger.PostRequest(GetIpv4Address(), dtoResponse);

            if (dtoResponse.Success == false)
            {
                return BadRequest(dtoResponse);
            }

            return Ok(dtoResponse);
        }

        // PUT api/<PublicMessagesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PublicMessagesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        #region PRIVATE METHODS

        private string GetIpv4Address()
        {
            return HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
        }

        #endregion
    }
}
