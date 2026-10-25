using System.ComponentModel.DataAnnotations;
using ePermitsApp.Constants;

namespace ePermits.DTOs
{
    public class RegisterDto
    {
        [Required]
        [StringLength(ValidationLimits.UsernameMaxLength, MinimumLength = ValidationLimits.UsernameMinLength)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationLimits.NameMaxLength)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationLimits.NameMaxLength)]
        public string MiddleName { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationLimits.NameMaxLength)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(ValidationLimits.EmailMaxLength)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string MobileNo { get; set; } = string.Empty;
    }
}