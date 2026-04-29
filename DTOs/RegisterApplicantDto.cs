using System.ComponentModel.DataAnnotations;

namespace ePermitsApp.DTOs
{
    public class RegisterApplicantDto
    {
        [Required]
        [MaxLength(100)]
        public string ApplicantName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? RepresentativeName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
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
