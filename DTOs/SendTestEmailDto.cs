using System.ComponentModel.DataAnnotations;

namespace ePermitsApp.DTOs;

public class SendTestEmailDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
