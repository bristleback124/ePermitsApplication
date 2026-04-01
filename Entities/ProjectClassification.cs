namespace ePermitsApp.Entities
{
    public class ProjectClassification
    {
        public int Id { get; set; }
        public string ProjectClassDesc { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
