    using System.Security.Claims;

namespace AuthService
{
    public interface ITokenManager
    {
        string GenerateAccessToken(int userId);
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }

}
