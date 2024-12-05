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
            // Получаем токен из заголовков запроса
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                // Создаем область для разрешения scoped сервисов
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    // Получаем ITokenManager в пределах области
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

