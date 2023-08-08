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

        //     _userConnectionService.RemoveConnectionAsync(userId, connectionId);

        //    await base.OnDisconnectedAsync(exception);
        //}
        public async Task SendMessage(Message message)
        {
            var userId = getUserId();
            var ConnectionId = await _userConnectionService.GetConnectionIdAsync(userId);

            if(ConnectionId != null) 
            {
                await Clients.Client(ConnectionId).SendAsync("ReceiveOne", message);
            }

        }

        public async Task SendEditedMessage(Message message)
        {
            var userId = getUserId();
            var ConnectionId = await _userConnectionService.GetConnectionIdAsync(userId);

            if (ConnectionId != null)
            {
                await Clients.Client(ConnectionId).SendAsync("ReceiveEdited", message);
            }

        }

        public async Task SendDeletedMessage(Message message)
        {
            var userId = getUserId();
            var ConnectionId = await _userConnectionService.GetConnectionIdAsync(userId);

            if (ConnectionId != null)
            {
                await Clients.Client(ConnectionId).SendAsync("ReceiveDeleted", message);
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

        public bool IsRedisConnected()
        {
            return _redisConnection != null && _redisConnection.IsConnected;
        }

    }
}

