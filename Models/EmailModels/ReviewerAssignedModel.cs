namespace ePermitsApp.Models.EmailModels;

public class ReviewerAssignedModel
{
    public string ReviewerName { get; set; } = string.Empty;
    public string ApplicationType { get; set; } = string.Empty;
    public string FormattedId { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
}
