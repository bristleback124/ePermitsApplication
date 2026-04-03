using ePermits.Models;
using ePermits.Models.DTOs;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class ApplicationNoteService : IApplicationNoteService
    {
        private readonly IApplicationNoteRepository _repository;

        public ApplicationNoteService(IApplicationNoteRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ApplicationNoteDto>> GetNotesAsync(int applicationId, int userId, string userRole)
        {
            var isInternal = userRole != "applicant";
            var notes = isInternal
                ? await _repository.GetByApplicationIdAsync(applicationId)
                : await _repository.GetVisibleByApplicationIdAsync(applicationId);

            return notes.Select(MapToDto);
        }

        public async Task<ApplicationNoteDto> CreateNoteAsync(CreateApplicationNoteDto dto, int userId)
        {
            var note = new ApplicationNote
            {
                ApplicationId = dto.ApplicationId,
                CreatedById = userId,
                Content = dto.Content,
                IsVisibleToApplicant = dto.IsVisibleToApplicant,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(note);

            // Reload with navigation properties
            var saved = await _repository.GetByIdAsync(note.Id);
            return MapToDto(saved!);
        }

        public async Task<ApplicationNoteDto?> UpdateVisibilityAsync(int noteId, bool isVisibleToApplicant)
        {
            var note = await _repository.GetByIdAsync(noteId);
            if (note == null) return null;

            note.IsVisibleToApplicant = isVisibleToApplicant;
            await _repository.UpdateAsync(note);

            return MapToDto(note);
        }

        private static ApplicationNoteDto MapToDto(ApplicationNote note)
        {
            var profile = note.CreatedBy?.UserProfile;
            var createdByName = profile != null
                ? $"{profile.FirstName} {profile.LastName}".Trim()
                : note.CreatedBy?.Username ?? "Unknown";

            return new ApplicationNoteDto
            {
                Id = note.Id,
                ApplicationId = note.ApplicationId,
                CreatedById = note.CreatedById,
                CreatedByName = createdByName,
                Content = note.Content,
                IsVisibleToApplicant = note.IsVisibleToApplicant,
                CreatedAt = note.CreatedAt
            };
        }
    }
}
