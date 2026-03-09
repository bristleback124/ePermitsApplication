using ePermitsApp.DTOs;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers
{
    [ApiController]
    [Route("api/applications")]
    [Authorize]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _service;

        public ApplicationsController(IApplicationService service)
        {
            _service = service;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ApplicationDtoShort>>> GetApplicationsByUserId(int userId)
        {
            var applications = await _service.GetApplicationsByUserIdAsync(userId);
            return Ok(applications);
        }

        [HttpGet("reviewer-dashboard")]
        public async Task<ActionResult<IEnumerable<ReviewerDashboardItemDto>>> GetReviewerDashboard()
        {
            var applications = await _service.GetReviewerDashboardAsync();
            return Ok(applications);
        }

        [HttpGet("status-options")]
        public async Task<ActionResult<ApplicationStatusOptionsDto>> GetStatusOptions()
        {
            var options = await _service.GetStatusOptionsAsync();
            return Ok(options);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("reviewers")]
        public async Task<ActionResult<IEnumerable<ReviewAssignableUserDto>>> GetAssignableReviewers([FromQuery] int departmentId)
        {
            var reviewers = await _service.GetAssignableReviewersAsync(departmentId);
            return Ok(reviewers);
        }

        [HttpGet("building-permit/{applicationId}")]
        public async Task<ActionResult<ApplicationBuildingPermitDetailDto>> GetApplicationBuildingPermitById(int applicationId)
        {
            var application = await _service.GetApplicationBuildingPermitById(applicationId);
            if (application == null)
                return NotFound();

            return Ok(application);
        }

        [HttpGet("coo/{applicationId}")]
        public async Task<ActionResult<ApplicationCoODetailDto>> GetApplicationCoOById(int applicationId)
        {
            var application = await _service.GetApplicationCoOById(applicationId);
            if (application == null)
                return NotFound();

            return Ok(application);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{applicationId}/reviews/{departmentId}/assign")]
        public async Task<ActionResult<ApplicationDepartmentReviewDto>> AssignReviewer(int applicationId, int departmentId, [FromBody] AssignApplicationReviewerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.AssignReviewerAsync(applicationId, departmentId, dto);
            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            return Ok(new { success = true, message = result.Message, data = result.Review });
        }

        [Authorize(Roles = "admin,user")]
        [HttpPut("{applicationId}/reviews/{departmentId}/status")]
        public async Task<ActionResult<ApplicationDepartmentReviewDto>> UpdateDepartmentReviewStatus(int applicationId, int departmentId, [FromBody] UpdateApplicationDepartmentReviewStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateDepartmentReviewStatusAsync(applicationId, departmentId, dto);
            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            return Ok(new { success = true, message = result.Message, data = result.Review });
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{applicationId}/overall-status")]
        public async Task<IActionResult> UpdateOverallStatus(int applicationId, [FromBody] UpdateApplicationOverallStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateOverallStatusAsync(applicationId, dto);
            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            return Ok(new { success = true, message = result.Message });
        }
    }
}
