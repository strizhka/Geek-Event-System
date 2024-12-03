namespace AuthService
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenManager _tokenManager;

        public TokenValidationMiddleware(RequestDelegate next, ITokenManager tokenManager)
        {
            _next = next;
            _tokenManager = tokenManager;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                var principal = _tokenManager.GetPrincipalFromToken(token);
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

            await _next(context);
        }
    }

}
