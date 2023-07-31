using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interfaces;
using WebApplication1.Modal;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {

        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        //GET: api/Message
        [HttpGet("{id}")]

        public async Task<IActionResult> GetMessage(string id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid request parameter." });
            }

            var messages = await _messageService.GetMessages(id);

            if (messages == null)
            {
                return NotFound(new { message = "User or conversation not found" });
            }

            return Ok(messages.Select(u => new
            {
                id = u.Id,
                senderId = u.SenderId,
                receiverId = u.ReceiverId,
                content = u.content,
                timestamp = u.Timestemp
            }));
        }

        // PUT: api/Message/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(int id, ContentRequest contentRequest)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(new { message = "message editing failed due to validation errors." });
            }

            return await _messageService.PutMessage(id, contentRequest);

        }

        // POST: api/Message
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "message sending failed due to validation errors." });
            }

            var messages = await _messageService.PostMessage(message);

            return Ok(messages);

        }

        // DELETE: api/Message/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            return await _messageService.DeleteMessage(id);
        }

        [HttpGet("/api/conversation/search/{result}")] 

        public async Task<IActionResult> SearchResult(string result)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "message sending failed due to validation errors." });
            }

            var message = await _messageService.GetMessageHistory(result);

            return Ok(message.Select(u => new
            {
                id = u.Id,
                senderId = u.SenderId,
                receiverId = u.ReceiverId,
                content = u.content,
                timestamp = u.Timestemp
            }));
        }
    }
}
