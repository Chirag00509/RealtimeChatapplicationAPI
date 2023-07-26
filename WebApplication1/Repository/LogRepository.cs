using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Modal;

namespace WebApplication1.Repository
{
    public class LogRepository : ILogRepository
    {
        private readonly ChatContext _context;
        public LogRepository(ChatContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<object>> GetLogs()
        {
            return await _context.Logs.Select(u => new
            {
                Id = u.id,
                Ip = u.Ip,
                Username = u.Username,
                RequestBody = u.RequestBody.Replace("\n", "").Replace("\"", "").Replace("\r", ""),
                TimeStamp = u.TimeStamp,
            }).ToListAsync();
        }
    }
}
