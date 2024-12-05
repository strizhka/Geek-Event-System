using AuthService.Models;
using Shared.Models;

namespace AuthService
{
    public interface IAuthManager
    {
        Task<string> RegisterUserAsync(RegisterRequest userDto);
        Task<string> LoginUserAsync(LoginRequest userDto);
        Task LogoutUserAsync(int userId);
        bool ValidatePassword(User user, string password);
        string HashPassword(User user, string password);
        string GenerateTemporaryPassword();
        Task SendRecoveryEmailAsync(string email, string temporaryPassword);
    }
}
