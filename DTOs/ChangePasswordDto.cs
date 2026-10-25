using System.ComponentModel.DataAnnotations;

namespace ePermits.DTOs
{
    public class ChangePasswordDto
    {
        // Required only when MustChangePassword is false (voluntary flow).
        // Skipped during forced first-login because the JWT already proves the user authenticated with their temp password.
        public string? CurrentPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// An admin setting another user's password (User Management), e.g. when they forgot it.
    public class AdminSetPasswordDto
    {
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
