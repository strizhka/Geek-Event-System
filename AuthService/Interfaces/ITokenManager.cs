    using System.Security.Claims;

namespace AuthService
{
    public interface ITokenManager
    {
        string GenerateAccessToken(int userId);
        string GenerateRefreshToken(int userId);
        (string AccessToken, string RefreshToken) GenerateTokens(int userId);
        Task<ClaimsPrincipal?> GetPrincipalFromToken(string token);
        int TokenLifetimeMinutes { get; }
    }
}
