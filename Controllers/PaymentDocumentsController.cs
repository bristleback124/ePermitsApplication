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
        [Authorize(Roles = "admin,superadmin,sysadmin,encoder,initial-reviewer,fee-assessor,final-reviewer,final-approver,releasing-officer")]
        public async Task<IActionResult> Upload(int applicationId, IFormFile file)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var document = await _service.UploadAsync(applicationId, file, userId);
            return Ok(document);
        }

        [HttpDelete("{documentId}")]
        [Authorize(Roles = "admin,superadmin,sysadmin,encoder,initial-reviewer,fee-assessor,final-reviewer,final-approver,releasing-officer")]
        public async Task<IActionResult> Delete(int documentId)
        {
            var result = await _service.DeleteAsync(documentId);
            if (!result) return NotFound();

            return Ok(new { success = true, message = "Payment document deleted." });
        }
    }
}
