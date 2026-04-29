using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ePermits.Controllers
{
    [Authorize(Roles = "admin,superadmin,sysadmin,user,encoder,initial-reviewer,technical-reviewer,fee-assessor,final-reviewer,final-approver")]
    [ApiController]
    [Route("api/audit-trail")]
    public class AuditTrailController : ControllerBase
    {
        private readonly IAuditTrailService _auditTrailService;

        public AuditTrailController(IAuditTrailService auditTrailService)
        {
            _auditTrailService = auditTrailService;
        }

        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetByApplicationId(int applicationId)
        {
            var auditTrails = await _auditTrailService.GetByApplicationIdAsync(applicationId);
            return Ok(auditTrails);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> UpdateDetails(int id, [FromBody] UpdateAuditDetailsDto dto)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(dto.Details))
                return BadRequest(new { success = false, message = "Details are required." });

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userId = int.TryParse(userIdClaim, out var uid) ? uid : 0;
            var userName = User.Identity?.Name ?? "System";

            var result = await _auditTrailService.UpdateDetailsAsync(id, dto.Details, userId, userName);
            if (!result.Success)
                return NotFound(new { success = false, message = result.Message });

            return Ok(new { success = true, message = result.Message });
        }

        [HttpGet("admin-activity")]
        [Authorize(Roles = "superadmin,sysadmin")]
        public async Task<IActionResult> GetAdminActivity()
        {
            var activity = await _auditTrailService.GetAdminActivityAsync();
            return Ok(new { success = true, data = activity });
        }
    }

    public class UpdateAuditDetailsDto
    {
        public string Details { get; set; } = string.Empty;
    }
}
