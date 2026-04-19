using System.ComponentModel.DataAnnotations;

namespace ePermits.DTOs
{
    public class UpdateUserAssignmentDto
    {
        [Required]
        public int UserRoleId { get; set; }

        public int? LGUId { get; set; }

        public int? DepartmentId { get; set; }
    }

    public class UserListItemDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string? LguName { get; set; }
        public int? LguId { get; set; }
        public string? DepartmentName { get; set; }
        public int? DepartmentId { get; set; }
    }
}
