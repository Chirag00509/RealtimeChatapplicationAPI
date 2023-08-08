using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Modal;

namespace WebApplication1.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatContext _context;

        public MessageRepository(ChatContext context)
        {
            _context = context;
        }

        public async Task<Message> AddMessage(Message message)
        {
            _context.Message.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task DeleteMessage(Message message)
        {
            _context.Remove(message);
            await _context.SaveChangesAsync();
        }

        public async Task<Message> GetMessageById(int id)
        {
            return await _context.Message.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<Message>> GetMessageHistory(string result)
        {
            return await _context.Message.Where(u => u.content.Contains(result)).ToListAsync();

        }

        public async Task<List<Message>> GetMessages(string currentUserId, string receiverId, int count, DateTime before)
        {
            var messages = await _context.Message
               .Where(u => (u.SenderId == currentUserId && u.ReceiverId == receiverId) ||
                           (u.SenderId == receiverId && u.ReceiverId == currentUserId))
               //&&
               //(u.Timestemp >= before))
               .OrderBy(u => u.Timestemp)
               .Take(count)
               .ToListAsync();

            return messages;
        }

        public async Task UpdateMessage(Message message)
        {
            _context.Message.Update(message);
            await _context.SaveChangesAsync();
        }
    }
}
