namespace ePermitsApp.Entities
{
    public class Province
    {
        public int Id { get; set; }
        public string ProvinceName { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public ICollection<LGU> LGUs { get; set; } = new List<LGU>();

        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
