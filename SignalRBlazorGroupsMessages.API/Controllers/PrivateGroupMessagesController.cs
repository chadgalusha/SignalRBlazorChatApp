using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SignalRBlazorGroupsMessages.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    [Authorize]
    public partial class PrivateGroupMessagesController : ControllerBase
    {
        private readonly IPrivateGroupMessagesService _service;
        private readonly ISerilogger _serilogger;

        public PrivateGroupMessagesController(IPrivateGroupMessagesService service, ISerilogger serilogger)
        {
            _service = service ?? throw new Exception(nameof(service));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        // GET: api/[controller]/bygroupid
        [HttpGet("bygroupid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<PrivateGroupMessageDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<PrivateGroupMessageDto>>>> GetListByGroupIdAsync(
            [FromQuery] int groupId, int numberItemsToSkip)
        {
            if (groupId < 0)
                return BadRequest("Invalid data: " + nameof(groupId));
            if (numberItemsToSkip < 0)
                return BadRequest("Invalid data: " + nameof(numberItemsToSkip));

            ApiResponse<List<PrivateGroupMessageDto>> dtoListResponse = await _service.GetDtoListByGroupIdAsync(groupId, numberItemsToSkip);
            _serilogger.GetRequest("0.0.0.0", dtoListResponse);

            switch (dtoListResponse.Message)
            {
                case "Group Id does not exists.":
                    return NotFound(dtoListResponse);
                case "Error getting messages.":
                    return StatusCode(StatusCodes.Status500InternalServerError, dtoListResponse);
                default:
                    return Ok(dtoListResponse);
            }
        }

        // GET: api/[controller]/byuserid
        [HttpGet("byuserid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<PrivateGroupMessageDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<PrivateGroupMessageDto>>>> GetListByUserIdAsync(
            [FromQuery] string userId, int numberItemsToSkip)
        {
            if (userId.IsNullOrEmpty())
                return BadRequest("Invalid data: " + nameof(userId));
            if (numberItemsToSkip < 0)
                return BadRequest("Invalid data: " + nameof(numberItemsToSkip));

            // if userId from JWT not userId in query, return bad request.
            string? jwtUserId = GetUserIdClaim();
            if (jwtUserId.IsNullOrEmpty())
            {
                return BadRequest("JWT userId not valid.");
            }
            if (jwtUserId != userId)
            {
                return BadRequest("JWT userId does not match query userId.");
            }

            ApiResponse<List<PrivateGroupMessageDto>> listDtoResponse = await _service.GetDtoListByUserIdAsync(userId, numberItemsToSkip);
            _serilogger.GetRequest("0.0.0.0", listDtoResponse);

            return Ok(listDtoResponse);
        }

        // GET api/[controller]/bymessageid
        [HttpGet("bymessageid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PrivateGroupMessageDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PrivateGroupMessageDto>>> GetByMessageIdAsync(
            [FromQuery] Guid messageId)
        {
            if (messageId == new Guid())
                return BadRequest("Invalid data: " + nameof(messageId));

            ApiResponse<PrivateGroupMessageDto> dtoResponse = await _service.GetDtoByMessageIdAsync(messageId);
            _serilogger.GetRequest("0.0.0.0", dtoResponse);

            switch (dtoResponse.Message)
            {
                case "Message Id not found.":
                    return NotFound(dtoResponse);
                case "Error getting message.":
                    return StatusCode(StatusCodes.Status500InternalServerError, dtoResponse);
                default:
                    return Ok(dtoResponse);
            }
        }

        // POST api/[controller]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PrivateGroupMessageDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PrivateGroupMessageDto>>> AddAsync([FromBody] CreatePrivateGroupMessageDto createDto)
        {
            if (!ModelState.IsValid || createDto == null)
            {
                return BadRequest(ModelState);
            }

            ApiResponse<PrivateGroupMessageDto> dtoResponse = await _service.AddAsync(createDto);
            _serilogger.PostRequest("0.0.0.0", dtoResponse);

            switch (dtoResponse.Message)
            {
                case string s when s.StartsWith("["):
                    return BadRequest(dtoResponse);
                case "Error saving new message.":
                    return StatusCode(StatusCodes.Status500InternalServerError, dtoResponse);
                default:
                    return Ok(dtoResponse);
            }
        }

        #region PRIVATE METHODS

        private string? GetUserIdClaim()
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                return identity.FindFirst("userId")?.Value;
            }

            return null;
        }

        #endregion
    }
}
