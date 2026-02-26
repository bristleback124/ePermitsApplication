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

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationDetailDto>> GetApplicationById(int id)
        {
            var application = await _service.GetApplicationByIdAsync(id);
            if (application == null)
                return NotFound();

            return Ok(application);
        }
    }
}
