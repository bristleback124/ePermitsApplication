using System.Security.Claims;
using ePermitsApp.DTOs;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers
{
    [ApiController]
    [Route("api/admin/system-utilities")]
    [Authorize(Roles = "superadmin")]
    public class SystemUtilitiesController : ControllerBase
    {
        private readonly ISystemUtilitiesService _service;

        public SystemUtilitiesController(ISystemUtilitiesService service)
        {
            _service = service;
        }

        [HttpPost("clear-applications")]
        public async Task<ActionResult<ClearApplicationsResultDto>> ClearApplications(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = int.TryParse(userIdClaim, out var uid) ? uid : 0;
            var userName = User.Identity?.Name ?? "System";

            var result = await _service.ClearAllApplicationsAsync(userId, userName, cancellationToken);
            return Ok(new { success = true, message = "All application records cleared.", data = result });
        }
    }
}
