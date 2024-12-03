using AuthService.Models;

namespace AuthService
{
    public interface IAuthManager
    {
        Task RegisterUserAsync(UserRegisterDto userDto);
        Task<string> LoginUserAsync(UserLoginDto userDto);
    }
}
