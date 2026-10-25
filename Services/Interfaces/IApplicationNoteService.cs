using ePermits.Models.DTOs;

namespace ePermitsApp.Services.Interfaces
{
    public interface IApplicationNoteService
    {
        Task<IEnumerable<ApplicationNoteDto>> GetNotesAsync(int applicationId, int userId, string userRole);
        Task<ApplicationNoteDto> CreateNoteAsync(CreateApplicationNoteDto dto, int userId);
        Task<ApplicationNoteDto?> UpdateVisibilityAsync(int noteId, bool isVisibleToApplicant);

        /// Opens one of a note's attachments, or returns null if it isn't on that note or the
        /// user can't see the note (applicants only see notes marked visible to them).
        Task<(Stream Content, string FileName)?> OpenAttachmentAsync(int noteId, string path, string userRole);
    }
}
