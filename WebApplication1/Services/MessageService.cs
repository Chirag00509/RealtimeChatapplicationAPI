using System.Security.Claims;
using WebApplication1.Interfaces;
using WebApplication1.Modal;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessageService(IMessageRepository messageRepository, IHttpContextAccessor httpContextAccessor)
        {
            _messageRepository = messageRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> DeleteMessage(int id)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var messages = await _messageRepository.GetMessageById(id);

            if (messages == null)
            {
                return new NotFoundObjectResult(new { message = "message not found" });
            }

            if (userId != messages.SenderId)
            {
                return new UnauthorizedObjectResult(new { message = "Unauthorized access" });
            }

            await _messageRepository.DeleteMessage(messages);

            return new OkObjectResult(new { message = "Message deleted successfully" });
        }

        public async Task<List<Message>> GetMessages(string receiverId)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var message = await _messageRepository.GetMessages(userId, receiverId);

            if (message == null)
            {
                return null;
            }

            return (message);
        }

        public async Task<MessageResponse> PostMessage(Message message)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return null;
            }

            message.SenderId = userId;
            message.Timestemp = DateTime.Now;

            var addMessage = await _messageRepository.AddMessage(message);

            var messageResponse = new MessageResponse
            {
                MessageId = addMessage.Id,
                SenderId = addMessage.SenderId,
                ReceiverId = addMessage.ReceiverId,
                Content = addMessage.content,
                Timestemp = addMessage.Timestemp,
            };

            return messageResponse;
        }

        public async Task<IActionResult> PutMessage(int id, ContentRequest ContentRequest)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var messages = await _messageRepository.GetMessageById(id);

            if (messages == null)
            {
                return new NotFoundObjectResult(new { message = "message not found" });
            }

            if (userId != messages.SenderId)
            {
                return new UnauthorizedObjectResult(new { message = "Unauthorized access" });
            }

            messages.content = ContentRequest.Content;
            await _messageRepository.UpdateMessage(messages);

            return new OkObjectResult(new { message = "Message edited successfully" });
        }
    }
}
