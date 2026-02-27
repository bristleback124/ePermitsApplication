using System.ComponentModel.DataAnnotations;

namespace ePermitsApp.DTOs
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public string UserRoleDesc { get; set; } = string.Empty;
    }

    public class CreateUserRoleDto
    {
        [Required]
        [StringLength(20)]
        public string UserRoleDesc { get; set; } = string.Empty;
    }

    public class UpdateUserRoleDto
    {
        [Required]
        [StringLength(20)]
        public string UserRoleDesc { get; set; } = string.Empty;
    }
}
