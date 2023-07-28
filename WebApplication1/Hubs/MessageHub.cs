using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace WebApplication1.Hubs
{
    public class MessageHub : Hub
    {
        public async Task BroadcastChartData(List<Message> data, string connectionId) =>
         await Clients.Client(connectionId).SendAsync("broadcastchartdata", data);
        public string GetConnectionId() => Context.ConnectionId;
    }
}

