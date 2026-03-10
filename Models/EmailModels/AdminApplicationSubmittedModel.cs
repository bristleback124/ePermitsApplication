namespace ePermitsApp.Models.EmailModels;

public class AdminApplicationSubmittedModel
{
    public string AdminName { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicationType { get; set; } = string.Empty;
    public string FormattedId { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
}
