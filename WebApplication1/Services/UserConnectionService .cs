using System.Collections.Concurrent;
using WebApplication1.Interfaces;

namespace WebApplication1.Services
{
    public class UserConnectionService : IUserConnectionService
    {
        private readonly ConcurrentDictionary<string, List<string>> _userConnections = new ConcurrentDictionary<string, List<string>>();

        public Task AddConnectionAsync(string userId, string connectionId)
        {
            var connections = _userConnections.GetOrAdd(userId, _ => new List<string>());
            connections.Add(connectionId);

            return Task.CompletedTask;
        }

        public Task<string> GetConnectionIdAsync(string userId)
        {
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                return Task.FromResult(connections.FirstOrDefault());
            }
            return Task.FromResult<string>(null);
        }

        public Task RemoveConnectionAsync(string userId, string connectionId)
        {
            throw new NotImplementedException();
        }
    }
}
