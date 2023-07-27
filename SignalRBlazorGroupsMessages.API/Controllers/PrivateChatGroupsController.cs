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
    public class PrivateChatGroupsController : ControllerBase
    {
        private readonly IPrivateChatGroupsService _service;
        private readonly ISerilogger _serilogger;
        private readonly IUserProvider _userProvider;

        public PrivateChatGroupsController(IPrivateChatGroupsService service, ISerilogger serilogger, IUserProvider userProvider)
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
            return apiResponse.Message switch
            {
                ErrorMessages.AddingItem => StatusCode(StatusCodes.Status500InternalServerError, apiResponse),
                ErrorMessages.AddingUser => StatusCode(StatusCodes.Status500InternalServerError, apiResponse),
                ErrorMessages.DeletingItem => StatusCode(StatusCodes.Status500InternalServerError, apiResponse),
                ErrorMessages.DeletingMessages => StatusCode(StatusCodes.Status500InternalServerError, apiResponse),
                ErrorMessages.Deletinguser => StatusCode(StatusCodes.Status500InternalServerError, apiResponse),
                ErrorMessages.GroupNameTaken => BadRequest(apiResponse),
                ErrorMessages.InvalidUserId => StatusCode(StatusCodes.Status403Forbidden, apiResponse),
                ErrorMessages.ModifyingItem => StatusCode(StatusCodes.Status500InternalServerError, apiResponse),
                ErrorMessages.NoModification => BadRequest(apiResponse),
                ErrorMessages.RecordNotFound => NotFound(apiResponse),
                ErrorMessages.RemovingUser => StatusCode(StatusCodes.Status500InternalServerError, apiResponse),
                ErrorMessages.RetrievingItems => StatusCode(StatusCodes.Status500InternalServerError, apiResponse),
                ErrorMessages.UserAlreadyInGroup => BadRequest(apiResponse),
                _ => NotFound(apiResponse)
            };
        }

        #endregion
    }
}
