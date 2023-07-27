using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;

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
            _userProvider = userProvider ?? throw new Exception(nameof(userProvider));
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

            ApiResponse<List<PrivateGroupMessageDto>> apiResponse = await _service.GetDtoListByGroupIdAsync(groupId, numberItemsToSkip);
            _serilogger.GetRequest("0.0.0.0", apiResponse);

            switch (apiResponse.Message)
            {
                case "Group Id does not exists.":
                    return NotFound(apiResponse);
                case "Error getting messages.":
                    return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
                default:
                    return Ok(apiResponse);
            }
        }

        // GET: api/[controller]/byuserid
        [HttpGet("byuserid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<PrivateGroupMessageDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<PrivateGroupMessageDto>>>> GetListByUserIdAsync(
            [FromQuery] string userId, int numberItemsToSkip)
        {
            if (userId.IsNullOrEmpty())
                return BadRequest("Invalid data: " + nameof(userId));
            if (numberItemsToSkip < 0)
                return BadRequest("Invalid data: " + nameof(numberItemsToSkip));

            // if userId from JWT not userId in query, return unauthorized.
            string? jwtUserId = GetJwtUserId(ControllerContext.HttpContext);
            if (!UserIdValid(jwtUserId, userId))
            {
                return Forbid("requesting userId not valid for this request.");
            }

            ApiResponse<List<PrivateGroupMessageDto>> apiResponse = await _service.GetDtoListByUserIdAsync(userId, numberItemsToSkip);
            _serilogger.GetRequest("0.0.0.0", apiResponse);

            switch (apiResponse.Message)
            {
                case "Error getting messages.":
                    return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
                default:
                    return Ok(apiResponse);
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
            ApiResponse<PrivateGroupMessageDto> apiResponse = await _service.GetDtoByMessageIdAsync(messageId);
            _serilogger.GetRequest("0.0.0.0", apiResponse);

            switch (apiResponse.Message)
            {
                case "Message Id not found.":
                    return NotFound(apiResponse);
                case "Error getting message.":
                    return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
                default:
                    return Ok(apiResponse);
            }
        }

        // POST api/[controller]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PrivateGroupMessageDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PrivateGroupMessageDto>>> AddAsync([FromBody] CreatePrivateGroupMessageDto createDto)
        {
            if (!ModelState.IsValid || createDto == null)
            {
                return BadRequest(ModelState);
            }

            // Check that jwt userId matches new message userId.
            string? jwtUserId = GetJwtUserId(ControllerContext.HttpContext);
            if (!UserIdValid(jwtUserId, createDto.UserId))
            {
                return Forbid("requesting userId not valid for this request.");
            }

            ApiResponse<PrivateGroupMessageDto> apiResponse = await _service.AddAsync(createDto);
            _serilogger.PostRequest("0.0.0.0", apiResponse);

            switch (apiResponse.Message)
            {
                case string s when s.StartsWith("["):
                    return BadRequest(apiResponse);
                case "Error saving new message.":
                    return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
                default:
                    return Ok(apiResponse);
            }
        }

        // PUT api/[controller]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PrivateGroupMessageDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PrivateGroupMessageDto>> ModifyAsync([FromBody] ModifyPrivateGroupMessageDto modifyDto)
        {
            if (!ModelState.IsValid || modifyDto == null)
            {
                return BadRequest(ModelState);
            }

            string? jwtUserId = GetJwtUserId(ControllerContext.HttpContext);
            if (!UserIdValid(jwtUserId))
            {
                return Forbid("requesting userId not valid for this request.");
            }

            ApiResponse<PrivateGroupMessageDto> apiResponse = await _service.ModifyAsync(modifyDto, jwtUserId!);
            _serilogger.PutRequest("0.0.0.0", apiResponse);

            switch (apiResponse.Message)
            {
                case string s when s.StartsWith("["):
                    return BadRequest(apiResponse);
                case "Error modifying message.":
                    return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
                default:
                    return Ok(apiResponse);
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PrivateGroupMessageDto>>> DeleteAsync([FromQuery] Guid messageId)
        {
            string? jwtUserId = GetJwtUserId(ControllerContext.HttpContext);
            if (!UserIdValid(jwtUserId))
            {
                return Forbid("Requesting userId not valid for this request.");
            }

            ApiResponse<PrivateGroupMessageDto> apiResponse = await _service.DeleteAsync(messageId, jwtUserId!);
            _serilogger.DeleteRequest("0.0.0.0", apiResponse);

            switch (apiResponse.Message)
            {
                case string s when s.StartsWith("["):
                    return BadRequest(apiResponse);
                case ("Response messages not deleted."):
                    return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
                case ("Error deleting message."):
                    return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
                default:
                    return NoContent();
            }
        }

        #region PRIVATE METHODS

        private string? GetJwtUserId(HttpContext context)
        {
            return _userProvider.GetUserIdClaim(context);
        }

        private bool UserIdValid(string? jwtUserId)
        {
            return jwtUserId.IsNullOrEmpty() ? false : true;
        }

        private bool UserIdValid(string? jwtUserId, string dtoUserId)
        {
            return jwtUserId.IsNullOrEmpty() == true || jwtUserId != dtoUserId ? false : true;
        }

        #endregion
    }
}
