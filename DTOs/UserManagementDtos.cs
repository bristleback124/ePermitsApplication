using System.ComponentModel.DataAnnotations;

namespace ePermits.DTOs
{
    public class UpdateUserAssignmentDto
    {
        // Single role (older clients). Ignored when UserRoleIds is non-empty.
        public int? UserRoleId { get; set; }

        // Every role the user should hold; validated against UserRoleRules.
        public List<int>? UserRoleIds { get; set; }

        public int? LGUId { get; set; }

        public int? DepartmentId { get; set; }
    }

    public class UserListItemDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int RoleId { get; set; }
        // Every role the user holds (Role/RoleId stay the primary role).
        public List<string> Roles { get; set; } = new List<string>();
        public List<int> RoleIds { get; set; } = new List<int>();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string? LguName { get; set; }
        public int? LguId { get; set; }
        public string? DepartmentName { get; set; }
        public int? DepartmentId { get; set; }
    }
}
