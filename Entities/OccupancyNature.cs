namespace ePermitsApp.Entities
{
    public class OccupancyNature
    {
        public int Id { get; set; }
        public string OccupancyNatureDesc { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
