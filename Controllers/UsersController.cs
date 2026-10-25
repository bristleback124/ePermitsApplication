using ePermits.DTOs;
using ePermits.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ePermits.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,superadmin,sysadmin,encoder")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UsersController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        /// Sets another user's password (e.g. they forgot it). Admin roles only, not encoders.
        [HttpPost("{id}/password")]
        [Authorize(Roles = "admin,superadmin,sysadmin")]
        public async Task<IActionResult> SetPassword(int id, [FromBody] AdminSetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });

            if (!int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var adminUserId))
                return Unauthorized(new { success = false, message = "Invalid token" });

            var adminRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;
            var (success, forbidden, message) = await _authService.SetPasswordByAdminAsync(id, dto.NewPassword, adminUserId, adminRole);

            if (forbidden)
                return StatusCode(StatusCodes.Status403Forbidden, new { success = false, message });

            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new { success = true, data = users });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            return Ok(new { success = true, data = user });
        }

        // Admins only: the class-level policy also admits encoders, who must not be able to grant roles.
        [HttpPut("{id}/role")]
        [Authorize(Roles = "admin,superadmin,sysadmin")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserAssignmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input data" });

            if (!int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var adminUserId))
                return Unauthorized(new { success = false, message = "Invalid token" });

            var adminRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;
            var (success, message) = await _userService.UpdateUserRoleAsync(id, dto, adminRole, adminUserId);

            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }

        [HttpGet("search-applicants")]
        [Authorize(Roles = "admin,superadmin,sysadmin,encoder")]
        public async Task<IActionResult> SearchApplicants([FromQuery] string? search)
        {
            var users = await _userService.SearchApplicantsAsync(search ?? "");
            return Ok(new { success = true, data = users });
        }
    }
}
