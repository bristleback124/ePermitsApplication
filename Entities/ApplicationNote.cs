using ePermitsApp.Entities;

namespace ePermits.Models
{
    public class ApplicationNote
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int CreatedById { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsVisibleToApplicant { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Application? Application { get; set; }
        public User? CreatedBy { get; set; }
    }
}
