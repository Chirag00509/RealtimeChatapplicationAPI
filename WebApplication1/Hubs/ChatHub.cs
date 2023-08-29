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
using StackExchange.Redis;
using WebApplication1.Modal;

namespace WebApplication1.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUserConnectionService _userConnectionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConnectionMultiplexer _redisConnection;


        public ChatHub(IUserConnectionService userConnectionService, IHttpContextAccessor httpContextAccessor, ConnectionMultiplexer multiplexer)
        {
            _userConnectionService = userConnectionService;
            _httpContextAccessor = httpContextAccessor;
            _redisConnection = multiplexer;
        }
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;

            var userId = getUserId();

            await _userConnectionService.AddConnectionAsync(userId, connectionId);

            await base.OnConnectedAsync();
        }

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    var connectionId = Context.ConnectionId;

        //    _userConnectionService.RemoveConnectionAsync(userId, connectionId);

        //    await base.OnDisconnectedAsync(exception);
        //}

        public async Task SendMessage(MessageDto message)
        {
            string userId = getUserId();

            var ConnectionId = await _userConnectionService.GetConnectionIdAsync(Convert.ToString(message.ReceiverId));

            if(ConnectionId != null) 
            {
                await Clients.Client(ConnectionId).SendAsync("ReceiveOne", message, userId);
            }
        }

        public async Task SendEditedMessage(MessageDto message)
        {
            var ConnectionId = await _userConnectionService.GetConnectionIdAsync(Convert.ToString(message.ReceiverId));

            if (ConnectionId != null)
            {
                await Clients.Client(ConnectionId).SendAsync("ReceiveEdited", message);
            }

        }

        public async Task SendDeletedMessage(MessageDto message)
        {
            string userId = Context.UserIdentifier;

            var ConnectionId = await _userConnectionService.GetConnectionIdAsync(userId);

            if (ConnectionId != null)
            {
                await Clients.Client(ConnectionId).SendAsync("ReceiveDeleted", message.ReceiverId);
            }
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

