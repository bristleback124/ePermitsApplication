using System.ComponentModel.DataAnnotations;
using ePermitsApp.Constants;

namespace ePermitsApp.DTOs
{
    public class RegisterApplicantDto
    {
        [Required]
        [MaxLength(ValidationLimits.NameMaxLength)]
        public string ApplicantName { get; set; } = string.Empty;

        [MaxLength(ValidationLimits.NameMaxLength)]
        public string? RepresentativeName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(ValidationLimits.EmailMaxLength)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string MobileNo { get; set; } = string.Empty;
    }

    public class RegisterApplicantResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
