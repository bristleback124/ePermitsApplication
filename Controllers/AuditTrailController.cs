using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ePermits.Controllers
{
    [Authorize(Roles = "admin,user")]
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
    }
}
