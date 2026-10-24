using ePermits.Models;
using ePermits.Models.DTOs;
using ePermitsApp.Helpers;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class ApplicationNoteService : IApplicationNoteService
    {
        // Upload slot name for note attachments (UploadedFileRefDto.Slot).
        public const string AttachmentsSlot = "attachments";

        private readonly IApplicationNoteRepository _repository;
        private readonly IPermitUploadService _uploads;
        private readonly IFileStorageService _storage;

        public ApplicationNoteService(IApplicationNoteRepository repository, IPermitUploadService uploads, IFileStorageService storage)
        {
            _repository = repository;
            _uploads = uploads;
            _storage = storage;
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
            var uploaded = await _uploads.ResolveAsync(dto.UploadedFiles, userId.ToString(), new[] { AttachmentsSlot });
            var attachments = uploaded.Take(AttachmentsSlot);
            uploaded.EnsureAllTaken();

            var note = new ApplicationNote
            {
                ApplicationId = dto.ApplicationId,
                CreatedById = userId,
                Content = dto.Content,
                IsVisibleToApplicant = dto.IsVisibleToApplicant,
                Attachments = attachments.Count > 0 ? FilePathHelper.Serialize(attachments) : null,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(note);
            await _uploads.MarkAttachedAsync(uploaded.Paths);

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

        public async Task<(Stream Content, string FileName)?> OpenAttachmentAsync(int noteId, string path, string userRole)
        {
            var note = await _repository.GetByIdAsync(noteId);
            if (note == null || (userRole == "applicant" && !note.IsVisibleToApplicant))
            {
                return null;
            }

            var attachment = FilePathHelper.Deserialize(note.Attachments)
                .FirstOrDefault(file => string.Equals(file.Path, path, StringComparison.Ordinal));
            if (attachment == null)
            {
                return null;
            }

            try
            {
                return (await _storage.DownloadAsync(attachment.Path), attachment.Name);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
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
                CreatedAt = note.CreatedAt,
                Attachments = FilePathHelper.Deserialize(note.Attachments)
            };
        }
    }
}
