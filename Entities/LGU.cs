namespace ePermitsApp.Entities
{
    public class LGU
    {
        public int Id { get; set; }
        public string LGUName { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public int ProvinceId { get; set; }
        public Province Province { get; set; } = null!;

        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
