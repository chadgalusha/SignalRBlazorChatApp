using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SignalRBlazorGroupsMessages.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    [Authorize]
    public partial class PrivateGroupMessagesController : ControllerBase
    {
        private readonly IPrivateGroupMessagesService _service;
        private readonly ISerilogger _serilogger;
        private readonly IUserProvider _userProvider;

        public PrivateGroupMessagesController(IPrivateGroupMessagesService service, ISerilogger serilogger, IUserProvider userProvider)
        {
            _service = service ?? throw new Exception(nameof(service));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
            _userProvider = userProvider;
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
            string? jwtUserId = _userProvider.GetUserIdClaim(ControllerContext.HttpContext);
            if (jwtUserId.IsNullOrEmpty())
            {
                return BadRequest("JWT userId not valid.");
            }
            if (jwtUserId != userId)
            {
                return BadRequest("JWT userId does not match query userId.");
            }

            ApiResponse<List<PrivateGroupMessageDto>> dtoListResponse = await _service.GetDtoListByUserIdAsync(userId, numberItemsToSkip);
            _serilogger.GetRequest("0.0.0.0", dtoListResponse);

            switch (dtoListResponse.Message)
            {
                case "Error getting messages.":
                    return StatusCode(StatusCodes.Status500InternalServerError, dtoListResponse);
                default:
                    return Ok(dtoListResponse);
            }
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

            // TODO: user validation here. create message should have same userid as jwt

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

        // PUT api/[controller]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PrivateGroupMessageDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PrivateGroupMessageDto>> ModifyAsync([FromBody] ModifyPrivateGroupMessageDto modifyDto)
        {
            if (!ModelState.IsValid || modifyDto == null)
            {
                return BadRequest(ModelState);
            }

            ApiResponse<PrivateGroupMessageDto> apiResponse = await _service.ModifyAsync(modifyDto);
            _serilogger.PutRequest("0.0.0.0", apiResponse);

            switch (apiResponse.Message)
            {
                case "Message Id not found.":
                    return NotFound(apiResponse);
                case "Error modifying message.":
                    return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
                default:
                    return Ok(apiResponse);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<PrivateGroupMessageDto>>> DeleteAsync([FromQuery] Guid messageId)
        {
            ApiResponse<PrivateGroupMessageDto> dtoResponse = await _service.DeleteAsync(messageId);
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
