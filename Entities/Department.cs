namespace ePermitsApp.Entities
{
    public class Department
    {
        public int Id { get; set; }

        public string DepartmentCode { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;

        public int LGUId { get; set; }
        public LGU LGU { get; set; } = null!;

        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
