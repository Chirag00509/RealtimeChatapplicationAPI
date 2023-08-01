using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Diagnostics;

namespace WebApplication1.Hubs
{
        public class ChatHub : Hub
        {
            public Task SendMessage(string user, Message message)
            {
                Console.WriteLine("Workinggg");
                return Clients.All.SendAsync("ReceiveOne", user, message);
            }
        }
}

