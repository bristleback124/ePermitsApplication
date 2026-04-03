namespace ePermitsApp.DTOs
{
    public class AuditTrailDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string PerformedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
