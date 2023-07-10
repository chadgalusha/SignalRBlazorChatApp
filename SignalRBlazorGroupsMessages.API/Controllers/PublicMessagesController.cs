using Microsoft.AspNetCore.Mvc;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorGroupsMessages.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class PublicMessagesController : ControllerBase
    {
        private readonly IPublicMessagesService _service;
        private readonly ISerilogger _serilogger;

        public PublicMessagesController(IPublicMessagesService service, ISerilogger serilogger)
        {
            _service    = service ?? throw new Exception(nameof(service));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        // GET: api/<PublicMessagesController>/bygroupid
        [HttpGet("bygroupid")]
        public async Task<ActionResult<ApiResponse<List<PublicMessageDto>>>> GetListByGroupIdAsync(
            [FromQuery] int groupId, int numberItemsToSkip)
        {
            if (groupId < 0)
                return BadRequest("Invalid data: " + nameof(groupId));
            if (numberItemsToSkip < 0)
                return BadRequest("Invalid data: "+nameof(numberItemsToSkip));

            ApiResponse<List<PublicMessageDto>> listDto = await _service.GetListByGroupIdAsync(groupId, numberItemsToSkip);

            return Ok(listDto);
        }

        // GET: api/<PublicMessagesController>/byuserid
        [HttpGet("byuserid")]
        public async Task<ActionResult<ApiResponse<List<PublicMessageDto>>>> GetListByUserIdAsync(
            [FromQuery] Guid userId, int numberItemsToSkip)
        {
            if (userId == new Guid())
                return BadRequest("Invalid data: " + nameof(userId));
            if (numberItemsToSkip < 0)
                return BadRequest("Invalid data: " + nameof(numberItemsToSkip));

            ApiResponse<List<PublicMessageDto>> listDto = await _service.GetViewListByUserIdAsync(userId, numberItemsToSkip);

            return Ok(listDto);
        }

        // GET api/<PublicMessagesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PublicMessagesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PublicMessagesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PublicMessagesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
