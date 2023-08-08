using StackExchange.Redis;
using WebApplication1.Interfaces;

namespace WebApplication1.Services
{
    public class UserConnectionService : IUserConnectionService
    {

        //private static ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();

        private readonly IDatabase _redisDb;

        //public UserConnectionService(ConnectionMultiplexer multiplexer)
        //{
        //    _redisDb = multiplexer.GetDatabase();
        //}

        public async Task AddConnectionAsync(string userId, string connectionId)
        {
            // _userConnections.AddOrUpdate(userId, connectionId, (_, existingConnectionId) => connectionId);

            //return Task.CompletedTask;

            await _redisDb.StringSetAsync(userId, connectionId);
        }

        public async void RemoveConnectionAsync(string userId, string connectionId)
        {
            //_userConnections.TryRemove(userId, out _);

            await _redisDb.KeyDeleteAsync(userId);
        }

        public async Task<string> GetConnectionIdAsync(string userId)
        {
            //if (_userConnections.TryGetValue(userId, out var connectionId))
            //{
            //    return connectionId;
            //}
            //return null;

            return await _redisDb.StringGetAsync(userId);
        }
    }
}
