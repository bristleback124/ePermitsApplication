namespace ePermitsApp.Entities
{
    public class RequirementCategory
    {
        public int Id { get; set; }
        public string ReqCatDesc { get; set; } = null!;

        public int ReqClassId { get; set; }
        public RequirementClassification RequirementClassification { get; set; } = null!;

        public ICollection<Requirement> Requirements { get; set; } = new List<Requirement>();

        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
