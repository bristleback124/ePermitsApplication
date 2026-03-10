using ePermits.Models;

namespace ePermitsApp.Entities;

public class AdminEmailNotificationConfig
{
    public int Id { get; set; }
    public string ApplicationType { get; set; } = string.Empty; // "BuildingPermit" or "CertificateOfOccupancy"
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public User? User { get; set; }
}
