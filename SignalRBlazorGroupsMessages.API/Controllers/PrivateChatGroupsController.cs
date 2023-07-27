using ChatApplicationModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class PrivateChatGroupsController : ControllerBase
    {
        private readonly IPrivateChatGroupsService _service;
        private readonly ISerilogger _serilogger;
        private readonly IUserProvider _userProvider;

        public PrivateChatGroupsController(IPrivateChatGroupsService service, ISerilogger serilogger, 
            IUserProvider userProvider)
        {
            _service = service ?? throw new Exception(nameof(service));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
            _userProvider = userProvider ?? throw new Exception(nameof(userProvider));
        }

        [HttpGet("byuserid")]
        public async Task<ActionResult<ApiResponse<List<PrivateChatGroupsDto>>>> GetDtoListByUserIdAsync([FromQuery] string userId)
        {
            ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new();

            string? jwtUserId = GetJwtUserId();
            if (!UserIdValid(jwtUserId, userId))
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, ErrorMessages.InvalidUserId);
                return ErrorHttpResponse(apiResponse);
            }

            apiResponse = await _service.GetDtoListByUserIdAsync(userId);
            _serilogger.GetRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }
            
            return Ok(apiResponse);
        }

        [HttpGet("bygroupid")]
        public async Task<ActionResult<ApiResponse<PrivateChatGroupsDto>>> GetDtoByGroupIdAsync([FromQuery] int groupid, string userId)
        {
            ApiResponse<PrivateChatGroupsDto> apiResponse = new();

            string? jwtUserId = GetJwtUserId();
            if (!UserIdValid(jwtUserId, userId))
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, ErrorMessages.InvalidUserId);
                return ErrorHttpResponse(apiResponse);
            }

            apiResponse = await _service.GetDtoByGroupIdAsync(groupid, userId);
            _serilogger.GetRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return Ok(apiResponse);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PrivateChatGroupsDto>>> AddAsync([FromBody] CreatePrivateChatGroupDto createDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            ApiResponse<PrivateChatGroupsDto> apiResponse = new();

            string? jwtUserId = GetJwtUserId();
            if (!UserIdValid(jwtUserId, createDto.GroupOwnerUserId))
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, ErrorMessages.InvalidUserId);
                return ErrorHttpResponse(apiResponse);
            }

            apiResponse = await _service.AddAsync(createDto);
            _serilogger.PostRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return Ok(apiResponse);
        }

        [HttpPost("groupmember")]
        public async Task<ActionResult<PrivateGroupMembers>> AddMemberAsync([FromQuery] int groupId, string userToAddId)
        {
            ApiResponse<PrivateGroupMembers> apiResponse = await _service.AddPrivateGroupMember(groupId, userToAddId);
            _serilogger.PostRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return Ok(apiResponse);
        }

        [HttpPut]
        public async Task<ActionResult<PrivateChatGroupsDto>> ModifyAsync([FromBody] ModifyPrivateChatGroupDto modifyDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            ApiResponse<PrivateChatGroupsDto> apiResponse = new();

            string? jwtUserId = GetJwtUserId();
            if (!UserIdValid(jwtUserId))
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, ErrorMessages.InvalidUserId);
                return ErrorHttpResponse(apiResponse);
            }

            apiResponse = await _service.ModifyAsync(modifyDto, jwtUserId!);
            _serilogger.PutRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return Ok(apiResponse);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAsync([FromQuery] int groupId)
        {
            ApiResponse<PrivateChatGroupsDto> apiResponse = new();

            string? jwtUserId = GetJwtUserId();
            if (!UserIdValid(jwtUserId))
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, ErrorMessages.InvalidUserId);
                return ErrorHttpResponse(apiResponse);
            }

            apiResponse = await _service.DeleteAsync(groupId, jwtUserId!);
            _serilogger.DeleteRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return NoContent();
        }

        [HttpDelete("groupmember")]
        public async Task<ActionResult> DeleteMemberAsync([FromQuery] int groupId, string userId)
        {
            ApiResponse<PrivateGroupMembers> apiResponse = await _service.RemoveUserFromGroupAsync(groupId, userId);
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
            return jwtUserId.IsNullOrEmpty() == true || jwtUserId != dtoUserId ? false : true;
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
