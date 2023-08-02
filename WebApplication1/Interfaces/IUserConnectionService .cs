namespace WebApplication1.Interfaces
{
    public interface IUserConnectionService
    {
        Task<string> GetConnectionIdAsync(string userId);
        Task AddConnectionAsync(string userId, string connectionId);
        Task RemoveConnectionAsync(string userId, string connectionId);
    }
}
