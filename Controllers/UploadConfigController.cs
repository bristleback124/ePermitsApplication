using ePermitsApp.Models;
using ePermitsApp.Services.Interfaces;
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
        private readonly IPermitUploadService _uploads;

        public UploadConfigController(IOptions<UploadSettings> settings, IPermitUploadService uploads)
        {
            _settings = settings.Value;
            _uploads = uploads;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                maxPermitUploadBytes = _settings.MaxPermitBytes,
                chunkedUpload = _uploads.IsEnabled,
                chunkSizeBytes = UploadSettings.ChunkSizeBytes,
            });
        }
    }
}
