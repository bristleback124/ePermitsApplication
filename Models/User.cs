using System;
using ePermitsApp.Entities;

namespace ePermits.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int UserRoleId { get; set; }
        public int? UserProfileId { get; set; }
        public int? LGUId { get; set; }
        public int? DepartmentId { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public UserRole? UserRole { get; set; }
        public UserProfile? UserProfile { get; set; }
        public LGU? LGU { get; set; }
        public Department? Department { get; set; }
    }
}