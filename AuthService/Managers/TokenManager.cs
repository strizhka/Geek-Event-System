using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace AuthService
{
    public class TokenManager : ITokenManager
    {
        private readonly string _secretKey;
        private readonly int _tokenLifetimeMinutes;
        private readonly AuthDbContext _context;

        public TokenManager(IOptions<TokenSettings> options, AuthDbContext context)
        {
            _secretKey = options.Value.SecretKey;
            _tokenLifetimeMinutes = options.Value.TokenLifetimeMinutes;
            _context = context;
        }


        public int TokenLifetimeMinutes => _tokenLifetimeMinutes;

        public (string AccessToken, string RefreshToken) GenerateTokens(int userId)
        {
            var accessToken = GenerateAccessToken(userId);
            var refreshToken = GenerateRefreshToken(userId);

            return (AccessToken: accessToken, RefreshToken: refreshToken);
        }


        public string GenerateAccessToken(int userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_tokenLifetimeMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(int userId)
        {
            var refreshToken = Guid.NewGuid().ToString();

            SaveRefreshToken(userId, refreshToken);

            return refreshToken;
        }

        private void SaveRefreshToken(int userId, string refreshToken)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                _context.SaveChangesAsync();
            }
        }

        public async Task<ClaimsPrincipal?> GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                var producer = new RabbitMqPublisher();
                await producer.SendValidationResultAsync(token, isValid: true);

                return principal;
            }
            catch
            {
                var producer = new RabbitMqPublisher();
                await producer.SendValidationResultAsync(token, isValid: false);

                return null;
            }
        }

    }

}
