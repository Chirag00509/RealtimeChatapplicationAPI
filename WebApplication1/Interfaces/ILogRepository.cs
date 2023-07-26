using WebApplication1.Modal;

namespace WebApplication1.Interfaces
{
    public interface ILogRepository
    {
        Task<IEnumerable<object>> GetLogs();

    }
}
