using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interfaces;
using WebApplication1.Modal;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;
        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        // GET: api/Log
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Logs>>> GetLogs(DateTime? startTime, DateTime? endTime)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request parameters" });
            }

            return await _logService.GetLogs(startTime, endTime);

        }
    }
}
