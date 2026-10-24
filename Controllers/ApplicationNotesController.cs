using ePermits.Models.DTOs;
using ePermitsApp.Helpers;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ePermits.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/application-notes")]
    public class ApplicationNotesController : ControllerBase
    {
        private readonly IApplicationNoteService _noteService;

        public ApplicationNotesController(IApplicationNoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetNotes(int applicationId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "applicant";

            var notes = await _noteService.GetNotesAsync(applicationId, userId, userRole);
            return Ok(notes);
        }

        [HttpPost]
        [Authorize(Roles = "admin,superadmin,sysadmin,encoder,initial-reviewer,technical-reviewer,fee-assessor,final-reviewer,final-approver,releasing-officer")]
        public async Task<IActionResult> CreateNote([FromBody] CreateApplicationNoteDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                var note = await _noteService.CreateNoteAsync(dto, userId);
                return Ok(note);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// One of a note's attachments. Applicants only get attachments of notes visible to them;
        /// anything else is a 404, so internal notes' files don't leak.
        [HttpGet("{noteId}/attachments")]
        public async Task<IActionResult> GetAttachment(int noteId, [FromQuery] string path, [FromQuery] bool download = false)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "applicant";
            var attachment = await _noteService.OpenAttachmentAsync(noteId, path, userRole);
            if (attachment == null) return NotFound();

            var (content, fileName) = attachment.Value;
            var contentType = FileContentTypes.FromFileName(fileName);
            return download ? File(content, contentType, fileDownloadName: fileName) : File(content, contentType);
        }

        [HttpPatch("{noteId}/visibility")]
        [Authorize(Roles = "admin,superadmin,sysadmin,encoder,initial-reviewer,technical-reviewer,fee-assessor,final-reviewer,final-approver,releasing-officer")]
        public async Task<IActionResult> UpdateVisibility(int noteId, [FromBody] UpdateNoteVisibilityDto dto)
        {
            var result = await _noteService.UpdateVisibilityAsync(noteId, dto.IsVisibleToApplicant);
            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}
