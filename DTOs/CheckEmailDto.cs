using System.ComponentModel.DataAnnotations;
using ePermitsApp.Constants;

namespace ePermits.DTOs
{
    public class CheckEmailDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(ValidationLimits.EmailMaxLength)]
        public string Email { get; set; } = string.Empty;
    }
}