using ePermits.Models;
using ePermitsApp.Data;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ePermits.Controllers
{
    [ApiController]
    [Route("api/clearance-documents")]
    [Authorize]
    public class ClearanceDocumentsController : ControllerBase
    {
        private const string InternalRoles = "admin,superadmin,sysadmin,user,encoder,initial-reviewer,fee-assessor,final-reviewer,final-approver,executive,releasing-officer";

        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileStorage;
        private readonly ICurrentUserService _currentUser;

        public ClearanceDocumentsController(
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
            var documents = await _context.ClearanceDocuments
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
        [Authorize(Roles = InternalRoles)]
        [RequestSizeLimit(50 * 1024 * 1024)]
        public async Task<IActionResult> Upload(
            int applicationId,
            [FromForm] List<IFormFile> files,
            [FromForm] string? description)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new { success = false, message = "No files provided." });

            var application = await _context.Applications.FindAsync(applicationId);
            if (application == null)
                return NotFound(new { success = false, message = "Application not found." });

            if (!int.TryParse(_currentUser.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not found." });

            foreach (var file in files)
            {
                if (file.Length == 0)
                    continue;

                var storedFileName = await _fileStorage.UploadAsync(file);

                _context.ClearanceDocuments.Add(new ClearanceDocument
                {
                    ApplicationId = applicationId,
                    UploadedById = userId,
                    FileName = file.FileName,
                    FileSize = file.Length,
                    FilePath = storedFileName,
                    Description = description,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"{files.Count} file(s) uploaded successfully."
            });
        }

        [HttpDelete("{documentId}")]
        [Authorize(Roles = InternalRoles)]
        public async Task<IActionResult> Delete(int documentId)
        {
            var document = await _context.ClearanceDocuments.FindAsync(documentId);
            if (document == null)
                return NotFound(new { success = false, message = "Document not found." });

            await _fileStorage.DeleteAsync(document.FilePath);
            _context.ClearanceDocuments.Remove(document);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Clearance document deleted." });
        }
    }
}
