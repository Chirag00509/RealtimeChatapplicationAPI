using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Diagnostics;

namespace WebApplication1.Hubs
{
        public class ChatHub : Hub
        {
            public Task SendMessage(Message message)
            {
                return Clients.All.SendAsync("ReceiveOne", message);
            }

            public Task SendEditedMessage(Message message) 
            {
                return Clients.All.SendAsync("ReceiveEdited", message);
            }

        }
}

