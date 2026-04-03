using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ePermits.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/payment-documents")]
    public class PaymentDocumentsController : ControllerBase
    {
        private readonly IPaymentDocumentService _service;

        public PaymentDocumentsController(IPaymentDocumentService service)
        {
            _service = service;
        }

        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetDocuments(int applicationId)
        {
            var documents = await _service.GetByApplicationIdAsync(applicationId);
            return Ok(documents);
        }

        [HttpPost("{applicationId}")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> Upload(int applicationId, IFormFile file)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "applicant";

            if (userRole == "applicant")
                return Forbid();

            var document = await _service.UploadAsync(applicationId, file, userId);
            return Ok(document);
        }

        [HttpDelete("{documentId}")]
        public async Task<IActionResult> Delete(int documentId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "applicant";

            if (userRole == "applicant")
                return Forbid();

            var result = await _service.DeleteAsync(documentId);
            if (!result) return NotFound();

            return Ok(new { success = true, message = "Payment document deleted." });
        }
    }
}
