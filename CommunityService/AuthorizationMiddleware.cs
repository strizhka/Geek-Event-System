using Microsoft.AspNetCore.Authorization;

namespace CommunityService
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() == null)
            {
                await _next(context);
                return;
            }

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Authorization header is missing or empty.");
                return;
            }

            if (!RabbitMqConsumer.IsTokenValid(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid or expired token.");
                return;
            }

            await _next(context);
        }
    }

}
