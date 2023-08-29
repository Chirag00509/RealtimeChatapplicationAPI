using System.Security.Claims;

namespace WebApplication1.Middlewares
{
    public class NewMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            var userName = context.User.FindFirst(ClaimTypes.Name)?.Value;

            string ip = context.Connection.RemoteIpAddress?.ToString();



        }
    }
}