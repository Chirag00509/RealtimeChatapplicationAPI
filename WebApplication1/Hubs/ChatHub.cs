using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace WebApplication1.Hubs
{
    public class ChatHub : Hub
    {
        public Task SendMessage1(string user, string message)
        {
            string connectionId = Context.ConnectionId;
            Console.WriteLine(connectionId);
            return Clients.All.SendAsync("ReceiveOne", user, message);


        }
    }
}

