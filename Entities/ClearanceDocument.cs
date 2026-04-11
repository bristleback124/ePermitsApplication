using ePermitsApp.Entities;

namespace ePermits.Models
{
    public class ClearanceDocument
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int UploadedById { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Application? Application { get; set; }
        public User? UploadedBy { get; set; }
    }
}
