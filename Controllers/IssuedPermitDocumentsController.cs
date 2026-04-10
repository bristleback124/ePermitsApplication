using ePermits.Models;
using ePermitsApp.Data;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ePermits.Controllers
{
    [ApiController]
    [Route("api/issued-permit-documents")]
    [Authorize]
    public class IssuedPermitDocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileStorage;
        private readonly ICurrentUserService _currentUser;

        public IssuedPermitDocumentsController(
            ApplicationDbContext context,
            IFileStorageService fileStorage,
            ICurrentUserService currentUser)
        {
            _context = context;
            _fileStorage = fileStorage;
            _currentUser = currentUser;
        }

        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetByApplicationId(int applicationId)
        {
            var documents = await _context.IssuedPermitDocuments
                .Where(d => d.ApplicationId == applicationId)
                .Include(d => d.UploadedBy)
                    .ThenInclude(u => u!.UserProfile)
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new
                {
                    d.Id,
                    d.ApplicationId,
                    d.FileName,
                    d.FileSize,
                    d.FilePath,
                    d.Description,
                    d.CreatedAt,
                    UploadedByName = d.UploadedBy != null && d.UploadedBy.UserProfile != null
                        ? d.UploadedBy.UserProfile.FirstName + " " + d.UploadedBy.UserProfile.LastName
                        : "Unknown"
                })
                .ToListAsync();

            return Ok(new { success = true, data = documents });
        }

        [HttpPost("{applicationId}")]
        [Authorize(Roles = "admin,superadmin,sysadmin,final-approver,encoder")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB
        public async Task<IActionResult> Upload(int applicationId, IFormFile file, [FromForm] string? description)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, message = "No file provided." });

            var application = await _context.Applications.FindAsync(applicationId);
            if (application == null)
                return NotFound(new { success = false, message = "Application not found." });

            if (!int.TryParse(_currentUser.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not found." });

            var storedFileName = await _fileStorage.UploadAsync(file);

            var document = new IssuedPermitDocument
            {
                ApplicationId = applicationId,
                UploadedById = userId,
                FileName = file.FileName,
                FileSize = file.Length,
                FilePath = storedFileName,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            _context.IssuedPermitDocuments.Add(document);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Permit document uploaded successfully.",
                data = new
                {
                    document.Id,
                    document.ApplicationId,
                    document.FileName,
                    document.FileSize,
                    document.FilePath,
                    document.Description,
                    document.CreatedAt
                }
            });
        }

        [HttpDelete("{documentId}")]
        [Authorize(Roles = "admin,superadmin,sysadmin,final-approver")]
        public async Task<IActionResult> Delete(int documentId)
        {
            var document = await _context.IssuedPermitDocuments.FindAsync(documentId);
            if (document == null)
                return NotFound(new { success = false, message = "Document not found." });

            await _fileStorage.DeleteAsync(document.FilePath);
            _context.IssuedPermitDocuments.Remove(document);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Document deleted." });
        }
    }
}
