namespace ePermitsApp.Entities
{
    public class PermitApplicationType
    {
        public int Id { get; set; }
        public string PermitAppTypeDesc { get; set; } = null!;

        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
