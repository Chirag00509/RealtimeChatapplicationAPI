namespace WebApplication1.Interfaces
{
    public interface IUserConnectionService
    {
        string GetConnectionIdAsync(string userId);
        Task AddConnectionAsync(string userId, string connectionId);
        void RemoveConnectionAsync(string userId, string connectionId);
    }
}
