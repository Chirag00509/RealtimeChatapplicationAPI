using System.Collections.Concurrent;
using WebApplication1.Interfaces;

namespace WebApplication1.Services
{
    public class UserConnectionService : IUserConnectionService
    {

        private static ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();

        public Task AddConnectionAsync(string userId, string connectionId)
        {
             _userConnections.AddOrUpdate(userId, connectionId, (_, existingConnectionId) => connectionId);

            return Task.CompletedTask;
        }

        public void RemoveConnectionAsync(string userId, string connectionId)
        {
            _userConnections.TryRemove(userId, out _);
        }

        public string GetConnectionIdAsync(string userId)
        {
            if (_userConnections.TryGetValue(userId, out var connectionId))
            {
                return connectionId;
            }
            return null;
        }
    }
}
