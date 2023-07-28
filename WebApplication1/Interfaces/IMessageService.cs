using Microsoft.AspNetCore.Mvc;
using WebApplication1.Modal;

namespace WebApplication1.Interfaces
{
    public interface IMessageService
    {
        Task<MessageResponse> PostMessage(Message message);
        Task<List<Message>> GetMessages(string receiverId);
        Task<IActionResult> PutMessage(int id, ContentRequest ContentRequest);
        Task<IActionResult> DeleteMessage(int id);

    }
}
