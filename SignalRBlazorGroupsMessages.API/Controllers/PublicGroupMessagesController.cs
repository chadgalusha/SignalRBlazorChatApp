using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorGroupsMessages.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    [Authorize]
    public class PublicGroupMessagesController : ControllerBase
    {
        private readonly IPublicGroupMessagesService _service;
        private readonly ISerilogger _serilogger;

        public PublicGroupMessagesController(IPublicGroupMessagesService service, ISerilogger serilogger)
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
            _serilogger.GetRequest("0.0.0.0", dtoList);

            return Ok(dtoList);
        }

        // GET: api/<PublicMessagesController>/byuserid
        [HttpGet("byuserid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<PublicGroupMessageDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<PublicGroupMessageDto>>>> GetListByUserIdAsync(
            [FromQuery] string userId, int numberItemsToSkip)
        {
            if (Guid.Parse(userId) == new Guid())
                return BadRequest("Invalid data: " + nameof(userId));
            if (numberItemsToSkip < 0)
                return BadRequest("Invalid data: " + nameof(numberItemsToSkip));

            ApiResponse<List<PublicGroupMessageDto>> listDtoResponse = await _service.GetListByUserIdAsync(userId, numberItemsToSkip);
            _serilogger.GetRequest("0.0.0.0", listDtoResponse);

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
            _serilogger.GetRequest("0.0.0.0", dtoResponse);

            switch (dtoResponse.Message)
            {
                case ("Message Id not found."):
                    return NotFound(dtoResponse);
                case ("Error getting message."):
                    return StatusCode(StatusCodes.Status500InternalServerError, dtoResponse);
                default:
                    return Ok(dtoResponse);
            }
        }

        // POST api/<PublicMessagesController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PublicGroupMessageDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PublicGroupMessageDto>>> AddAsync([FromBody] PublicGroupMessageDto dtoToCreate)
        {
            if (!ModelState.IsValid || dtoToCreate == null)
            {
                return BadRequest(ModelState);
            }

            ApiResponse<PublicGroupMessageDto> dtoResponse = await _service.AddAsync(dtoToCreate);
            _serilogger.PostRequest("0.0.0.0", dtoResponse);

            switch (dtoResponse.Message)
            {
                case ("Error saving new message."):
                    return StatusCode(StatusCodes.Status500InternalServerError, dtoResponse);
                default:
                    return Ok(dtoResponse);
            }
        }

        // PUT api/<PublicMessagesController>/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PublicGroupMessageDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PublicGroupMessageDto>>> ModifyAsync([FromBody] ModifyPublicGroupMessageDto dtoToModify)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApiResponse<PublicGroupMessageDto> dtoResponse = await _service.ModifyAsync(dtoToModify);
            _serilogger.PutRequest("0.0.0.0", dtoResponse);

            switch (dtoResponse.Message)
            {
                case ("Message Id not found."):
                    return NotFound(dtoResponse);
                case ("Error modifying message."):
                    return StatusCode(StatusCodes.Status500InternalServerError, dtoResponse);
                default:
                    return Ok(dtoResponse);
            }
        }

        // DELETE api/<PublicMessagesController>/5
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PublicGroupMessageDto>>> DeleteAsync([FromQuery] Guid messageId)
        {
            ApiResponse<PublicGroupMessageDto> dtoResponse = await _service.DeleteAsync(messageId);
            _serilogger.DeleteRequest("0.0.0.0", dtoResponse);

            switch (dtoResponse.Message)
            {
                case ("Message Id not found."):
                    return NotFound(dtoResponse);
                case ("Response messages not deleted."):
                    return StatusCode(StatusCodes.Status500InternalServerError, dtoResponse);
                case ("Error deleting message."):
                    return StatusCode(StatusCodes.Status500InternalServerError, dtoResponse);
                default:
                    return NoContent();
            }
        }

        #region PRIVATE METHODS

        private string GetIpv4Address()
        {
            return HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
        }

        #endregion
    }
}
