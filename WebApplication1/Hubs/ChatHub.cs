using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using NuGet.Common;
using System.Diagnostics;
using System.Security.Claims;
using WebApplication1.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;


namespace WebApplication1.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUserConnectionService _userConnectionService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ChatHub(IUserConnectionService userConnectionService, IHttpContextAccessor httpContextAccessor)
        {
            _userConnectionService = userConnectionService;
            _httpContextAccessor = httpContextAccessor;
        }
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;

            var userId = getUserId();

           _userConnectionService.AddConnectionAsync(userId, connectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var connectionId = Context.ConnectionId;

             _userConnectionService.RemoveConnectionAsync(userId, connectionId);

            await base.OnDisconnectedAsync(exception);
        }
        public Task SendMessage(Message message)
        {
            var userId = getUserId();
            var ConnectionId = _userConnectionService.GetConnectionIdAsync(userId);

            if(ConnectionId == null) 
            {
                return null;
            }

            return Clients.Client(ConnectionId).SendAsync("ReceiveOne", message);
        }

        public Task SendEditedMessage(Message message) 
        {
            return Clients.All.SendAsync("ReceiveEdited", message);
        }

        public Task SendDeletedMessage(Message message) 
        {
            return Clients.All.SendAsync("ReceiveDeleted", message);
        }

        public string getUserId()
        {
            var token = Context.GetHttpContext().Request.Query["token"].ToString();

            var jwtToken = new JwtSecurityToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            var id = userIdClaim.Value;

            return id;
        }

    }
}

