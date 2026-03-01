using ePermits.Models;

namespace ePermitsApp.Entities
{
    public class ApplicationDepartmentReview
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int DepartmentId { get; set; }
        public string Status { get; set; } = "In Queue";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Application? Application { get; set; }
        public Department? Department { get; set; }
    }
}
