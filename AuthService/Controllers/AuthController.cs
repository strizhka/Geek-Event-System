using AuthService.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly AuthDbContext _context;
        private readonly ITokenManager _tokenManager;

        public AuthController(IAuthManager authManager, AuthDbContext context, ITokenManager tokenManager)
        {
            _authManager = authManager;
            _context = context;
            _tokenManager = tokenManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest userDto)
        {
            try
            {
                await _authManager.RegisterUserAsync(userDto);
                return Ok("User registered successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Models.LoginRequest userDto)
        {
            try
            {
                var token = await _authManager.LoginUserAsync(userDto);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest logoutRequest)
        {
            try
            {
                await _authManager.LogoutUserAsync(logoutRequest.UserId);
                return Ok("User logged out successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var refreshToken = refreshTokenRequest.RefreshToken;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                return Unauthorized("Invalid refresh token.");
            }

            var newTokens = _tokenManager.GenerateTokens(user.UserId);
            user.RefreshToken = newTokens.RefreshToken;
            await _context.SaveChangesAsync();

            return Ok(newTokens);
        }
    }
}