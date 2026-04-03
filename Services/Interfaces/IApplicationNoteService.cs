using ePermits.Models.DTOs;

namespace ePermitsApp.Services.Interfaces
{
    public interface IApplicationNoteService
    {
        Task<IEnumerable<ApplicationNoteDto>> GetNotesAsync(int applicationId, int userId, string userRole);
        Task<ApplicationNoteDto> CreateNoteAsync(CreateApplicationNoteDto dto, int userId);
        Task<ApplicationNoteDto?> UpdateVisibilityAsync(int noteId, bool isVisibleToApplicant);
    }
}
