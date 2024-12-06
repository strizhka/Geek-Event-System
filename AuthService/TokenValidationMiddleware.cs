using Microsoft.AspNetCore.Authorization;

namespace AuthService
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TokenValidationMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {

            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() == null)
            {
                await _next(context);
                return;
            }

            Console.WriteLine($"Вызвался middleware");
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"токен пустой!");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Authorization header is missing or empty.");
                return;
            }
            else
            {
                Console.WriteLine($"Received token: {token}");
            }

            if (token != null)
            {
                Console.WriteLine($"токен есть!");
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var tokenManager = scope.ServiceProvider.GetRequiredService<ITokenManager>();

                    var principal = await tokenManager.GetPrincipalFromToken(token);
                    if (principal != null)
                    {
                        context.User = principal;
                        var producer = new RabbitMqPublisher();
                        await producer.SendValidationResultAsync(token, isValid: true);
                    }
                    else
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Invalid token.");
                        var producer = new RabbitMqPublisher();
                        await producer.SendValidationResultAsync(token, isValid: false);
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}

