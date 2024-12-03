
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Shared.Models;
using Microsoft.EntityFrameworkCore;
using AuthService.Models;

namespace AuthService
{
    public class AuthManager : IAuthManager
    {
        private readonly AuthDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenManager _tokenManager;

        public AuthManager(IPasswordHasher<User> passwordHasher, AuthDbContext context, ITokenManager tokenManager)
        {
            _passwordHasher = passwordHasher;
            _context = context;
            _tokenManager = tokenManager;
        }

        public async Task RegisterUserAsync(UserRegisterDto userDto)
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

            user.HashedPassword = _passwordHasher.HashPassword(user, userDto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<string> LoginUserAsync(UserLoginDto userDto)
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
    }


}
