using System.ComponentModel.DataAnnotations;
using ePermitsApp.Constants;

namespace ePermits.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        [StringLength(ValidationLimits.EmailMaxLength)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}