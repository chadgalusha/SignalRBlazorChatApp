﻿using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<ApiResponse<List<PrivateGroupMessageDto>>>> GetListByGroupIdAsync(
            [FromQuery] int groupId, [FromQuery] int numberMessagesToSkip)
        {
            ApiResponse<List<PrivateGroupMessageDto>> apiResponse = new();

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

            apiResponse = await _service.GetDtoListByGroupIdAsync(groupId, numberMessagesToSkip);
            _serilogger.GetRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success) 
            { 
                return ErrorHttpResponse(apiResponse); 
            }

            return Ok(apiResponse);
        }

        // GET: api/[controller]/byuserid
        [HttpGet("byuserid")]
        public async Task<ActionResult<ApiResponse<List<PrivateGroupMessageDto>>>> GetListByUserIdAsync(
            [FromQuery] string userId, [FromQuery] int numberMessagesToSkip)
        {
            ApiResponse<List<PrivateGroupMessageDto>> apiResponse = new();

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

            // if userId from JWT not userId in query, return unauthorized.
            string? jwtUserId = GetJwtUserId();
            if (!UserIdValid(jwtUserId, userId))
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, ErrorMessages.InvalidUserId);
                return ErrorHttpResponse(apiResponse);
            }

            apiResponse = await _service.GetDtoListByUserIdAsync(userId, numberMessagesToSkip);
            _serilogger.GetRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success) 
            { 
                return ErrorHttpResponse(apiResponse);
            }

            return Ok(apiResponse);
        }

        // GET api/[controller]/bymessageid
        [HttpGet("bymessageid")]
        public async Task<ActionResult<ApiResponse<PrivateGroupMessageDto>>> GetByMessageIdAsync(
            [FromQuery] Guid messageId)
        {
            ApiResponse<PrivateGroupMessageDto> apiResponse = await _service.GetDtoByMessageIdAsync(messageId);
            _serilogger.GetRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return Ok(apiResponse);
        }

        // POST api/[controller]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<PrivateGroupMessageDto>>> AddAsync([FromBody] CreatePrivateGroupMessageDto createDto)
        {
            ApiResponse<PrivateGroupMessageDto> apiResponse = new();

            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            // Check that jwt userId matches new message userId.
            string? jwtUserId = GetJwtUserId();
            if (!UserIdValid(jwtUserId, createDto.UserId))
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, ErrorMessages.InvalidUserId);
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

        // PUT api/[controller]
        [HttpPut]
        public async Task<ActionResult<PrivateGroupMessageDto>> ModifyAsync([FromBody] ModifyPrivateGroupMessageDto modifyDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            ApiResponse<PrivateGroupMessageDto> apiResponse = new();

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
        public async Task<ActionResult<ApiResponse<PrivateGroupMessageDto>>> DeleteAsync([FromQuery] Guid privateMessageId)
        {
            ApiResponse<PrivateGroupMessageDto> apiResponse = new();

            string? jwtUserId = GetJwtUserId();
            if (!UserIdValid(jwtUserId))
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, ErrorMessages.InvalidUserId);
                return ErrorHttpResponse(apiResponse);
            }

            apiResponse = await _service.DeleteAsync(privateMessageId, jwtUserId!);
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
