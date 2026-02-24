namespace ePermitsApp.Entities
{
    public class Requirement
    {
        public int Id { get; set; }
        public string ReqDesc { get; set; } = null!;

        public int ReqCatId { get; set; }
        public RequirementCategory RequirementCategory { get; set; } = null!;

        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
