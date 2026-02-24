namespace ePermitsApp.Entities
{
    public class Barangay
    {
        public int Id { get; set; }
        public string BarangayName { get; set; } = null!;

        public int LGUId { get; set; }
        public LGU LGU { get; set; } = null!;

        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
