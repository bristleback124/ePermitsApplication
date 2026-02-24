using ePermits.DTOs;
using ePermits.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ePermits.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        /// Register a new user
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = "Invalid input data", 
                    errors = ModelState 
                });
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (result == null)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = "Registration failed. Username or email already exists." 
                });
            }

            return Ok(new 
            { 
                success = true, 
                message = "Registration successful", 
                data = result 
            });
        }

        /// Login user with email and password
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = "Invalid input data", 
                    errors = ModelState 
                });
            }

            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized(new 
                { 
                    success = false, 
                    message = "Invalid email or password" 
                });
            }

            return Ok(new 
            { 
                success = true, 
                message = "Login successful", 
                data = result 
            });
        }

        /// Validate token (for testing purposes)
        [HttpPost("validate")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ValidateToken([FromBody] string token)
        {
            var isValid = await _authService.ValidateTokenAsync(token);

            if (!isValid)
            {
                return Unauthorized(new 
                { 
                    success = false, 
                    message = "Invalid or expired token" 
                });
            }

            return Ok(new 
            { 
                success = true, 
                message = "Token is valid" 
            });
        }

        /// Get current user info (requires authentication)
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { success = false, message = "Invalid token" });

            var userInfo = await _authService.GetUserInfoAsync(userId);

            if (userInfo == null)
                return NotFound(new { success = false, message = "User not found" });

            return Ok(new { success = true, data = userInfo });
        }

        [HttpPost("check-email")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckEmail([FromBody] CheckEmailDto checkEmailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = "Invalid input data", 
                    errors = ModelState 
                });
            }

            var emailExists = await _authService.CheckEmailExistsAsync(checkEmailDto.Email);

            return Ok(new 
            { 
                success = true,
                exists = emailExists,
                message = emailExists ? "Email found" : "User does not exist"
            });
        }

    }
}