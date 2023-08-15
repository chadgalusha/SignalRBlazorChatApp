using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
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
        private readonly IUserProvider _userProvider;

        public PublicGroupMessagesController(IPublicGroupMessagesService service, ISerilogger serilogger, IUserProvider userProvider)
        {
            _service = service ?? throw new Exception(nameof(service));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
            _userProvider = userProvider ?? throw new Exception(nameof(userProvider));
        }

        // GET: api/<PublicMessagesController>/bygroupid
        [HttpGet("bygroupid")]
        public async Task<ActionResult<ApiResponse<List<PublicGroupMessageDto>>>> GetListByGroupIdAsync(
            [FromQuery] int groupId, int numberMessagesToSkip)
        {
            ApiResponse<List<PublicGroupMessageDto>> apiResponse = new();

            if (groupId < 0)
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, "Invalid data: " + nameof(groupId));
                return BadRequest(apiResponse);
            }
            if (numberMessagesToSkip < 0)
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, "Invalid data: " + nameof(numberMessagesToSkip));
                return BadRequest(apiResponse);
            }

            apiResponse = await _service.GetListByGroupIdAsync(groupId, numberMessagesToSkip);
            _serilogger.GetRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return Ok(apiResponse);
        }

        // GET: api/<PublicMessagesController>/byuserid
        [HttpGet("byuserid")]
        public async Task<ActionResult<ApiResponse<List<PublicGroupMessageDto>>>> GetListByUserIdAsync(
            [FromQuery] string userId, int numberMessagesToSkip)
        {
            ApiResponse<List<PublicGroupMessageDto>> apiResponse = new();

            if (userId.IsNullOrEmpty())
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, "Invalid data: " + nameof(userId));
                return BadRequest(apiResponse);
            }
            if (numberMessagesToSkip < 0)
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, "Invalid data: " + nameof(numberMessagesToSkip));
                return BadRequest(apiResponse);
            }

            apiResponse = await _service.GetListByUserIdAsync(userId, numberMessagesToSkip);
            _serilogger.GetRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return Ok(apiResponse);
        }

        // GET api/<PublicMessagesController>/bymessageid
        [HttpGet("bymessageid")]
        public async Task<ActionResult<ApiResponse<PublicGroupMessageDto>>> GetByMessageIdAsync(
            [FromQuery] Guid messageId)
        {
            ApiResponse<PublicGroupMessageDto>  apiResponse = await _service.GetByMessageIdAsync(messageId);
            _serilogger.GetRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return Ok(apiResponse);
        }

        // POST api/<PublicMessagesController>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<PublicGroupMessageDto>>> AddAsync([FromBody] CreatePublicGroupMessageDto createDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            ApiResponse<PublicGroupMessageDto> apiResponse = new();

            string? jwtUserId = GetJwtUserId();
            if (!UserIdValid(jwtUserId, createDto.UserId))
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, ErrorMessages.InvalidUserId);
                Log.Information($"jsonUserId is {jwtUserId} and userId is {createDto.UserId}");
                return ErrorHttpResponse(apiResponse);
            }
            if (createDto.Text.IsNullOrEmpty())
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, "Invalid item: " + nameof(createDto.Text));
                return BadRequest(apiResponse);
            }

            apiResponse = await _service.AddAsync(createDto);
            _serilogger.PostRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return Ok(apiResponse);
        }

        // PUT api/<PublicMessagesController>/
        [HttpPut]
        public async Task<ActionResult<ApiResponse<PublicGroupMessageDto>>> ModifyAsync([FromBody] ModifyPublicGroupMessageDto dtoToModify)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            ApiResponse<PublicGroupMessageDto> apiResponse = new();

            string? jwtUserId = GetJwtUserId();
            if (!UserIdValid(jwtUserId))
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, ErrorMessages.InvalidUserId);
                return ErrorHttpResponse(apiResponse);
            }

            apiResponse = await _service.ModifyAsync(dtoToModify, jwtUserId!);
            _serilogger.PutRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return Ok(apiResponse);
        }

        // DELETE api/<PublicMessagesController>/5
        [HttpDelete]
        public async Task<ActionResult<ApiResponse<PublicGroupMessageDto>>> DeleteAsync([FromQuery] Guid messageId)
        {
            ApiResponse<PublicGroupMessageDto> apiResponse = new();

            string? jwtUserId = GetJwtUserId();
            if (!UserIdValid(jwtUserId))
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, ErrorMessages.InvalidUserId);
                return ErrorHttpResponse(apiResponse);
            }

            apiResponse = await _service.DeleteAsync(messageId, jwtUserId!);
            _serilogger.DeleteRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return NoContent();
        }

        #region PRIVATE METHODS

        private string? GetJwtUserId()
        {
            return _userProvider.GetUserIdClaim(ControllerContext.HttpContext);
        }

        private bool UserIdValid(string? jwtUserId)
        {
            return jwtUserId.IsNullOrEmpty() ? false : true;
        }

        private bool UserIdValid(string? jwtUserId, string dtoUserId)
        {
            return jwtUserId == dtoUserId;
        }

        private string GetUserIp()
        {
            return _userProvider.GetUserIP(ControllerContext.HttpContext);
        }

        private ActionResult ErrorHttpResponse<T>(ApiResponse<T> apiResponse)
        {
            int errorCode = HttpErrorCodes.Get(apiResponse.Message);
            return StatusCode(errorCode, apiResponse);
        }

        #endregion
    }
}
