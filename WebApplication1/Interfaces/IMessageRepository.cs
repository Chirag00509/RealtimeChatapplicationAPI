using WebApplication1.Modal;

namespace WebApplication1.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> AddMessage(Message message);

        Task<List<Message>> GetMessages(string currentUserId ,string receiverId);
        Task<Message> GetMessageById(int id);
        Task UpdateMessage(Message message);
        Task DeleteMessage(Message message);
    }
}
