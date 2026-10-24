using ePermitsApp.DTOs;
using ePermitsApp.Models;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers
{
    /// Chunked uploads of BP/CoO document files: start, send each chunk, complete. Each request is
    /// small, so file size isn't limited by the request size cap or the App Service timeout.
    [ApiController]
    [Route("api/uploads/permit-files")]
    [Authorize]
    public class UploadsController : ControllerBase
    {
        public const string TokenHeader = "X-Upload-Token";

        private readonly IPermitUploadService _uploads;
        private readonly ICurrentUserService _currentUser;

        public UploadsController(IPermitUploadService uploads, ICurrentUserService currentUser)
        {
            _uploads = uploads;
            _currentUser = currentUser;
        }

        [HttpPost]
        public ActionResult<StartUploadResponseDto> Start([FromBody] StartUploadRequestDto request) =>
            Handle(userId => Ok(_uploads.Start(request.FileName, userId)));

        /// Body: the raw bytes of chunk `index` (0-based), at most UploadSettings.ChunkSizeBytes.
        [HttpPut("chunks")]
        [RequestSizeLimit(UploadSettings.ChunkSizeBytes + 1024 * 1024)]
        public Task<IActionResult> StageChunk(
            [FromQuery] string path,
            [FromQuery] int index,
            [FromHeader(Name = TokenHeader)] string? token,
            CancellationToken cancellationToken) =>
            HandleAsync(async userId =>
            {
                await _uploads.StageChunkAsync(path, token ?? string.Empty, userId, index, Request.Body, cancellationToken);
                return NoContent();
            });

        [HttpPost("complete")]
        public Task<IActionResult> Complete([FromBody] CompleteUploadRequestDto request, CancellationToken cancellationToken) =>
            HandleAsync(async userId => Ok(await _uploads.CompleteAsync(request, userId, cancellationToken)));

        private ActionResult<StartUploadResponseDto> Handle(Func<string, ActionResult<StartUploadResponseDto>> action)
        {
            if (!_uploads.IsEnabled) return NotFound(new { success = false, message = "Chunked uploads are not enabled." });
            if (string.IsNullOrWhiteSpace(_currentUser.UserId)) return Unauthorized();

            try
            {
                return action(_currentUser.UserId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private async Task<IActionResult> HandleAsync(Func<string, Task<IActionResult>> action)
        {
            if (!_uploads.IsEnabled) return NotFound(new { success = false, message = "Chunked uploads are not enabled." });
            if (string.IsNullOrWhiteSpace(_currentUser.UserId)) return Unauthorized();

            try
            {
                return await action(_currentUser.UserId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
