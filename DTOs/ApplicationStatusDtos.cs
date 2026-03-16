using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ePermitsApp.DTOs
{
    public class ApplicationStatusOptionsDto
    {
        public List<string> OverallStatuses { get; set; } = new();
        public List<string> DepartmentStatuses { get; set; } = new();
    }

    public class AssignApplicationReviewerDto
    {
        [Required]
        public int ReviewerId { get; set; }
    }

    public class UpdateApplicationDepartmentReviewStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateApplicationOverallStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }

    public class ReviewAssignableUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public bool IsPlaceholder { get; set; }
    }
}
