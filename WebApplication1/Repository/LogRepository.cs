using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        public async Task<IEnumerable<object>> GetLogs(DateTime? customStartTime, DateTime? customEndTime)
        {
            IEnumerable<Logs> loggs;

            if (customEndTime.HasValue)
            {
                loggs = _context.Logs
                 .Where(log => log.TimeStamp >= customStartTime && log.TimeStamp <= customEndTime);
            }
            else
            {
                loggs =  _context.Logs.Where(log => log.TimeStamp <= customStartTime);
            }

            return loggs
                .Select(u => new
            {
                Id = u.id,
                Ip = u.Ip,
                Username = u.Username,
                RequestBody = u.RequestBody.Replace("\n", "").Replace("\"", "").Replace("\r", ""),
                TimeStamp = u.TimeStamp,
            });
        }
    }
}
