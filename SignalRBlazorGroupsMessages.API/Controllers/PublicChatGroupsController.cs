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
    public class PublicChatGroupsController : ControllerBase
    {
        private readonly IPublicChatGroupsService _service;
        private readonly ISerilogger _serilogger;

        public PublicChatGroupsController(IPublicChatGroupsService service, ISerilogger serilogger)
        {
            _service = service ?? throw new Exception(nameof(service));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PublicChatGroupsDto>>>> GetPublicChatGroupsAsync()
        {
            ApiResponse<List<PublicChatGroupsDto>> dtoList = await _service.GetListPublicChatGroupsAsync();
            _serilogger.GetRequest("0.0.0.0", dtoList);

            return Ok(dtoList);
        }

        [HttpGet("byid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PublicChatGroupsDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PublicChatGroupsDto>>> GetByIdAsync(
            [FromQuery] int groupId)
        {
            if (groupId < 0)
                return BadRequest("Invalid data: " + nameof(groupId));

            ApiResponse<PublicChatGroupsDto> dtoResponse = await _service.GetViewByIdAsync(groupId);
            _serilogger.GetRequest("0.0.0.0", dtoResponse);

            return Ok(dtoResponse);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PublicChatGroupsDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PublicChatGroupsDto>>> AddAsync([FromBody] CreatePublicChatGroupDto dtoToCreate)
        {
            if (!ModelState.IsValid || !CreateDtoChecks(dtoToCreate))
            {
                return BadRequest(ModelState);
            }

            ApiResponse<PublicChatGroupsDto> dtoResponse = await _service.AddAsync(dtoToCreate);
            _serilogger.PostRequest("0.0.0.0", dtoResponse);

            return Ok(dtoResponse);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PublicChatGroupsDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PublicChatGroupsDto>>> ModifyAsync([FromBody] ModifyPublicChatGroupDto dtoToModify)
        {
            if (!ModelState.IsValid || dtoToModify == null)
            {
                return BadRequest(ModelState);
            }

            ApiResponse<PublicChatGroupsDto> dtoResponse = await _service.ModifyAsync(dtoToModify);
            _serilogger.PutRequest("0.0.0.0", dtoResponse);

            return Ok(dtoResponse);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PublicChatGroupsDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PublicChatGroupsDto>>> DeleteAsync([FromQuery] int groupId)
        {
            ApiResponse<PublicChatGroupsDto> dtoResponse = await _service.DeleteAsync(groupId);
            _serilogger.DeleteRequest("0.0.0.0", dtoResponse);

            switch (dtoResponse.Message)
            {
                case ("Chat Group Id not found"):
                    return NotFound(dtoResponse);
                case ("Error deleting messages from this group"):
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

        private bool CreateDtoChecks(CreatePublicChatGroupDto createDto)
        {
            bool result = true;

            if (createDto == null)
                result = false;
            if (createDto.ChatGroupName.IsNullOrEmpty())
                result = false;
            if (createDto.GroupOwnerUserId == new Guid())
                result = false;
            return result;
        }

        #endregion
    }
}
