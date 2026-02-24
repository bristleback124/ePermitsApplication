using System;

namespace ePermits.Models
{
    public class Application
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Applicant
        public string Type { get; set; } = string.Empty; // "BuildingPermit" or "CertificateOfOccupancy"
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, etc.
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}
