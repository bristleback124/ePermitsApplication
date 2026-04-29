using ePermitsApp.DTOs;
using ePermitsApp.Models.EmailModels;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-test")]
        public async Task<IActionResult> SendTestEmail([FromBody] SendTestEmailDto dto)
        {
            var model = new ApplicationSubmittedModel
            {
                ApplicantName = "Test Applicant",
                ApplicationType = "Building Permit",
                FormattedId = "BP-01-26-01-0001",
                SubmittedAt = DateTime.Now
            };

            await _emailService.SendTemplatedEmailAsync(
                dto.Email,
                "ePermits Test Email",
                "ApplicationSubmitted",
                model);

            return Ok(new { success = true, message = $"Test email sent to {dto.Email}" });
        }
    }
}
