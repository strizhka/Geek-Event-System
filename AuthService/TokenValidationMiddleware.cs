namespace AuthService
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RabbitMqConsumer _consumer;

        public TokenValidationMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory, RabbitMqConsumer consumer)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
            _consumer = consumer;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
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
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var tokenManager = scope.ServiceProvider.GetRequiredService<ITokenManager>();

                    var principal = await tokenManager.GetPrincipalFromToken(token);
                    if (principal != null)
                    {
                        context.User = principal;
                    }
                    else
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Invalid token.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}

