using AuthService.Models;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
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
        [Authorize]
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

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(new { user.Email, user.UserName, user.UserRole });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var isValid = _authManager.ValidatePassword(user, request.CurrentPassword);

            if (!isValid)
            {
                return BadRequest("Invalid current password.");
            }

            user.HashedPassword = _authManager.HashPassword(user, request.NewPassword);
            await _context.SaveChangesAsync();

            return Ok("Password changed successfully.");
        }

        [HttpPost("recover-password")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null)
                {
                    return BadRequest("User with this email does not exist.");
                }

                var temporaryPassword = _authManager.GenerateTemporaryPassword();

                user.HashedPassword = _authManager.HashPassword(user, temporaryPassword);

                await _context.SaveChangesAsync();

                await _authManager.SendRecoveryEmailAsync(user.Email, temporaryPassword);

                return Ok("Temporary password sent to your email.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("delete-account")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(request.UserId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (userIdClaim == null || userIdClaim != user.UserId.ToString())
                {
                    return Forbid("You do not have permission to delete this account.");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok("User account deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the account: {ex.Message}");
            }
        }
    }
}