namespace ePermitsApp.Entities
{
    public class OwnershipType
    {
        public int Id { get; set; }
        public string OwnershipTypeDesc { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
