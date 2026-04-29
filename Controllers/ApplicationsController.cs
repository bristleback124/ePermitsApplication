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
        private readonly IApplicationPdfService _pdfService;

        public ApplicationsController(IApplicationService service, IApplicationPdfService pdfService)
        {
            _service = service;
            _pdfService = pdfService;
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

        [Authorize(Roles = "admin,superadmin,sysadmin,encoder,initial-reviewer,technical-reviewer,fee-assessor,final-reviewer,final-approver")]
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

        [Authorize(Roles = "admin,superadmin,sysadmin,encoder,initial-reviewer,technical-reviewer,fee-assessor,final-reviewer,final-approver")]
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

        [Authorize(Roles = "admin,superadmin,sysadmin,user,encoder,initial-reviewer,technical-reviewer,fee-assessor,final-reviewer,final-approver")]
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

        [Authorize(Roles = "admin,superadmin,sysadmin,encoder,initial-reviewer,technical-reviewer,fee-assessor,final-reviewer,final-approver,applicant")]
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

        [Authorize(Roles = "initial-reviewer,technical-reviewer")]
        [HttpPut("{applicationId}/review-substatus")]
        public async Task<IActionResult> UpdateReviewSubstatus(int applicationId, [FromBody] UpdateApplicationReviewSubstatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateReviewSubstatusAsync(applicationId, dto);
            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            return Ok(new { success = true, message = result.Message });
        }

        [HttpGet("{applicationId}/download-all")]
        public async Task<IActionResult> DownloadAll(int applicationId, [FromQuery] string type)
        {
            if (type != "BuildingPermit" && type != "COO")
                return BadRequest(new { success = false, message = "Invalid application type. Must be 'BuildingPermit' or 'COO'." });

            try
            {
                var pdfBytes = await _pdfService.GenerateApplicationPdfAsync(applicationId, type);
                return File(pdfBytes, "application/pdf", $"Application-{applicationId}-Documents.pdf");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }
    }
}
