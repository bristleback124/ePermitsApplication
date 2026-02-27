using ePermitsApp.DTOs;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers
{
    [ApiController]
    [Route("api/applications")]
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
    }
}
