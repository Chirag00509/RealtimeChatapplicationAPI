using Microsoft.AspNetCore.Mvc;
using WebApplication1.Modal;

namespace WebApplication1.Interfaces
{
    public interface ILogService
    {
        Task<ActionResult<IEnumerable<Logs>>> GetLogs(DateTime? startTime, DateTime? endTime);
    }
}
