using System.Security.Claims;
using WebApplication1.Interfaces;
using WebApplication1.Modal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Hubs;

namespace WebApplication1.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUserConnectionService _userConnectionService;

        public MessageService(IMessageRepository messageRepository, IHttpContextAccessor httpContextAccessor, IHubContext<ChatHub> hubContext, IUserConnectionService userConnectionService)
        {
            _messageRepository = messageRepository;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
            _userConnectionService = userConnectionService;
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

            var senderConnectionId = await _userConnectionService.GetConnectionIdAsync(userId);
            var receiverConnectionId = await _userConnectionService.GetConnectionIdAsync(messages.ReceiverId);

            if (senderConnectionId != null)
            {
                await _hubContext.Clients.Client(senderConnectionId).SendAsync("ReceiveDeleted", messages);
            }

            if (receiverConnectionId != null)
            {
                await _hubContext.Clients.Client(receiverConnectionId).SendAsync("ReceiveDeleted", messages);
            }

            return new OkObjectResult(new { message = "Message deleted successfully" });
        }

        public async Task<List<Message>> GetMessageHistory(string result)
        {
            var getHistory = await _messageRepository.GetMessageHistory(result);

            return getHistory;


        }

        public async Task<List<Message>> GetMessages(string receiverId, int count, DateTime before)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var message = await _messageRepository.GetMessages(userId, receiverId, count, before);

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


            var senderConnectionId = await _userConnectionService.GetConnectionIdAsync(userId);
            var receiverConnectionId = await _userConnectionService.GetConnectionIdAsync(message.ReceiverId);

            if (senderConnectionId != null)
            {
                await _hubContext.Clients.Client(senderConnectionId).SendAsync("ReceiveOne", message);
            }

            if (receiverConnectionId != null)
            {
                await _hubContext.Clients.Client(receiverConnectionId).SendAsync("ReceiveOne", message);
            }

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

        public async Task<IActionResult> PutMessage(int id, Message message)
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

            messages.content = message.content;
            await _messageRepository.UpdateMessage(messages);

            var senderConnectionId = await _userConnectionService.GetConnectionIdAsync(userId);
            var receiverConnectionId = await _userConnectionService.GetConnectionIdAsync(messages.ReceiverId);

            if (senderConnectionId != null)
            {
                await _hubContext.Clients.Client(senderConnectionId).SendAsync("ReceiveEdited", messages);
            }

            if (receiverConnectionId != null)
            {
                await _hubContext.Clients.Client(receiverConnectionId).SendAsync("ReceiveEdited", messages);
            }

            return new OkObjectResult(new { message = "Message edited successfully" });
        }
    }
}
