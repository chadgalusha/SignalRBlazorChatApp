using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorGroupsMessages.API.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    [Authorize]
    public class PublicChatGroupsController : ControllerBase
    {
        private readonly IPublicChatGroupsService _service;
        private readonly ISerilogger _serilogger;
        private readonly IUserProvider _userProvider;

        public PublicChatGroupsController(IPublicChatGroupsService service, ISerilogger serilogger, 
            IUserProvider userProvider)
        {
            _service = service ?? throw new Exception(nameof(service));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
            _userProvider = userProvider ?? throw new Exception(nameof(userProvider));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PublicChatGroupsDto>>>> GetPublicChatGroupsAsync()
        {
            ApiResponse<List<PublicChatGroupsDto>> apiResponse = await _service.GetListAsync();
            _serilogger.GetRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return Ok(apiResponse);
        }

        [HttpGet("byid")]
        public async Task<ActionResult<ApiResponse<PublicChatGroupsDto>>> GetByIdAsync(
            [FromQuery] int groupId)
        {
            ApiResponse<PublicChatGroupsDto> apiResponse = new();

            if (groupId < 0)
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, "Invalid data: " + nameof(groupId));
                return BadRequest(apiResponse);
            }

            apiResponse = await _service.GetByIdAsync(groupId);
            _serilogger.GetRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return Ok(apiResponse);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PublicChatGroupsDto>>> AddAsync([FromBody] CreatePublicChatGroupDto dtoToCreate)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            ApiResponse<PublicChatGroupsDto> apiResponse = new();

            string? jwtUserId = GetJwtUserId();
            if (!UserIdValid(jwtUserId, dtoToCreate.GroupOwnerUserId))
            {
                apiResponse = ReturnApiResponse.Failure(apiResponse, ErrorMessages.InvalidUserId);
                return BadRequest(apiResponse);
            }

            apiResponse = await _service.AddAsync(dtoToCreate);
            _serilogger.PostRequest(GetUserIp(), apiResponse);

            if (!apiResponse.Success)
            {
                return ErrorHttpResponse(apiResponse);
            }

            return Ok(apiResponse);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<PublicChatGroupsDto>>> ModifyAsync([FromBody] ModifyPublicChatGroupDto dtoToModify)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            ApiResponse<PublicChatGroupsDto> apiResponse = new();

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

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<PublicChatGroupsDto>>> DeleteAsync([FromQuery] int groupId)
        {
            ApiResponse<PublicChatGroupsDto> apiResponse = new();

            string ? jwtUserId = GetJwtUserId();
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
