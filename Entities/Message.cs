using System;

namespace ePermits.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int SenderId { get; set; }
        public string SenderType { get; set; } = string.Empty; // "Applicant" or "Admin"
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        // Navigation properties
        public Application? Application { get; set; }
        public User? Sender { get; set; }
    }
}
