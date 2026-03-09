namespace ePermitsApp.Models.EmailModels;

public class ApplicationStatusUpdatedModel
{
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicationType { get; set; } = string.Empty;
    public string FormattedId { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}
