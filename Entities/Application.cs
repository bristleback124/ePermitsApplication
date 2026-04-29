using ePermitsApp.Entities.BuildingPermit;
using ePermitsApp.Entities.CoOApp;

namespace ePermits.Models
{
    public class Application
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Applicant
        public string Type { get; set; } = string.Empty; // "BuildingPermit" or "CertificateOfOccupancy"
        public string FormattedId { get; set; } = string.Empty;
        public int? SubmittedById { get; set; } // Encoder who submitted on behalf of applicant (null = self-submitted)
        public string Status { get; set; } = "Submitted";
        public string? StatusReason { get; set; } // Reason for reject/cancel/deficiency/reopen
        public string RequirementsReviewStatus { get; set; } = "Review Not Started";
        public string TechnicalPlansReviewStatus { get; set; } = "Review Not Started";
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public User? User { get; set; } // Applicant
        public User? SubmittedBy { get; set; } // Encoder (if submitted on behalf)
        public BuildingPermit? BuildingPermit { get; set; }
        public CoOApp? CoOApp { get; set; }
        public ICollection<Message>? Messages { get; set; }
        public ICollection<ePermitsApp.Entities.ApplicationDepartmentReview> DepartmentReviews { get; set; } = new List<ePermitsApp.Entities.ApplicationDepartmentReview>();
    }
}
