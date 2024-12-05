using AuthService.Models;

namespace AuthService
{
    public interface IAuthManager
    {
        Task<string> RegisterUserAsync(RegisterRequest userDto);
        Task<string> LoginUserAsync(LoginRequest userDto);

        Task LogoutUserAsync(int userId);
    }
}
