namespace ePermitsApp.Entities
{
    public class RequirementClassification
    {
        public int Id { get; set; }
        public string ReqClassDesc { get; set; } = null!;
        public string ApplicationTypeScope { get; set; } = "Both";
        public bool IsActive { get; set; } = true;

        public ICollection<RequirementCategory> RequirementCategorys { get; set; } = new List<RequirementCategory>();

        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
