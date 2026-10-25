namespace ePermits.Models
{
    public class AuditTrail
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Details { get; set; }
        // No FK to User — intentional denormalization. Captures name at event time.
        public int PerformedByUserId { get; set; }
        public string PerformedByName { get; set; } = string.Empty;
        // Role the action was taken as (the workflow transition's RequiredRole); null for non-role actions and older rows.
        public string? PerformedByRole { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Application? Application { get; set; }
    }
}
