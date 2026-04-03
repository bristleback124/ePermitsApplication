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
    }

    public class CreateApplicationNoteDto
    {
        public int ApplicationId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsVisibleToApplicant { get; set; } = false;
    }

    public class UpdateNoteVisibilityDto
    {
        public bool IsVisibleToApplicant { get; set; }
    }
}
