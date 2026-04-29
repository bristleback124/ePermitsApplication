using ePermits.Models.DTOs;
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
        [Authorize(Roles = "admin,superadmin,sysadmin,encoder,initial-reviewer,technical-reviewer,fee-assessor,final-reviewer,final-approver")]
        public async Task<IActionResult> CreateNote([FromBody] CreateApplicationNoteDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var note = await _noteService.CreateNoteAsync(dto, userId);
            return Ok(note);
        }

        [HttpPatch("{noteId}/visibility")]
        [Authorize(Roles = "admin,superadmin,sysadmin,encoder,initial-reviewer,technical-reviewer,fee-assessor,final-reviewer,final-approver")]
        public async Task<IActionResult> UpdateVisibility(int noteId, [FromBody] UpdateNoteVisibilityDto dto)
        {
            var result = await _noteService.UpdateVisibilityAsync(noteId, dto.IsVisibleToApplicant);
            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}
