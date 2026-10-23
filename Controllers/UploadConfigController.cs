using ePermitsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ePermitsApp.Controllers
{
    [ApiController]
    [Route("api/upload-config")]
    [Authorize]
    public class UploadConfigController : ControllerBase
    {
        private readonly UploadSettings _settings;

        public UploadConfigController(IOptions<UploadSettings> settings)
        {
            _settings = settings.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { maxPermitUploadBytes = _settings.MaxPermitBytes });
        }
    }
}
