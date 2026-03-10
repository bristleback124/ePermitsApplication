namespace ePermitsApp.Models.EmailModels;

public class ApplicationStatusUpdatedModel
{
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicationType { get; set; } = string.Empty;
    public string FormattedId { get; set; } = string.Empty;
    public string? PreviousStatus { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
    public string? DepartmentName { get; set; }
    public string? DepartmentCode { get; set; }
    public string? UpdatedBy { get; set; }
}
