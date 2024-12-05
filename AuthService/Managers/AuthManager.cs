using Microsoft.AspNetCore.Identity;
using Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using AuthService.Interfaces;

namespace AuthService
{
    public class AuthManager : IAuthManager
    {
        private readonly AuthDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenManager _tokenManager;
        private readonly IEmailManager _emailManager;

        public AuthManager(IPasswordHasher<User> passwordHasher, AuthDbContext context, ITokenManager tokenManager, IEmailManager emailManager)
        {
            _passwordHasher = passwordHasher;
            _context = context;
            _tokenManager = tokenManager;
            _emailManager = emailManager;
        }

        public async Task<string> RegisterUserAsync(RegisterRequest userDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
            {
                throw new InvalidOperationException("User already exists.");
            }

            var user = new User
            {
                Email = userDto.Email,
                UserName = userDto.Username ?? userDto.Email,
                UserRole = "User"
            };

            user.HashedPassword = HashPassword(user, userDto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var tokens = _tokenManager.GenerateTokens(user.UserId);
            return JsonConvert.SerializeObject(tokens);
        }

        public async Task<string> LoginUserAsync(Models.LoginRequest userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, userDto.Password);
            if (result != PasswordVerificationResult.Success)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var token = _tokenManager.GenerateAccessToken(user.UserId);
            return token;
        }

        public async Task LogoutUserAsync(int userId)
        {
            // Удаляем Refresh Token из базы данных
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                user.RefreshToken = null; // Удаляем Refresh Token
                await _context.SaveChangesAsync();
            }

            //// Опционально отправляем уведомление через RabbitMQ
            //var producer = new RabbitMqProducer();
            //await producer.SendLogoutNotificationAsync(token);
        }

        public bool ValidatePassword(User user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task SendRecoveryEmailAsync(string email, string temporaryPassword)
        {
            var subject = "Password Recovery";
            var body = $"Your temporary password is: {temporaryPassword}";
            await _emailManager.SendEmailAsync(email, subject, body);
        }
    }
}
