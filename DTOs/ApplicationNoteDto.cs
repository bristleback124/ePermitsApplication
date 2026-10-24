using ePermitsApp.DTOs;

namespace ePermits.Models.DTOs
{
    public class ApplicationNoteDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsVisibleToApplicant { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<FileMetadataDto> Attachments { get; set; } = new();
    }

    public class CreateApplicationNoteDto
    {
        public int ApplicationId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsVisibleToApplicant { get; set; } = false;

        /// Files uploaded in chunks via /api/uploads/permit-files, with slot "attachments".
        public List<UploadedFileRefDto> UploadedFiles { get; set; } = new();
    }

    public class UpdateNoteVisibilityDto
    {
        public bool IsVisibleToApplicant { get; set; }
    }
}
