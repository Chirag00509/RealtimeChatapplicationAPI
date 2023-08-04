using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interfaces;
using WebApplication1.Modal;
using WebApplication1.Repository;

namespace WebApplication1.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }
        public async Task<ActionResult<IEnumerable<Logs>>> GetLogs(DateTime? startTime, DateTime? endTime)
        {
            var loggs = await _logRepository.GetLogs(startTime, endTime); 
                
            if (loggs == null)
            {
                return new NotFoundObjectResult(new { message = "Logs not found" });
            }

            return new OkObjectResult(loggs);
        }
    }
}
