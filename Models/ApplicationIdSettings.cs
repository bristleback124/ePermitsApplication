namespace ePermitsApp.Models;

public class ApplicationIdSettings
{
    public PermitIdSettings BuildingPermit { get; set; } = new();
    public PermitIdSettings CertificateOfOccupancy { get; set; } = new();
}

public class PermitIdSettings
{
    public int LegacySequenceOffset { get; set; }
    public int LegacyYear { get; set; }
}
