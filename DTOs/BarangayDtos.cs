namespace ePermitsApp.DTOs
{
    public class BarangayDto
    {
        public int Id { get; init; }
        public string BarangayName { get; init; } = null!;
        public int LGUId { get; init; }
        public string LGUName { get; init; } = null!;
        public string ProvinceName { get; init; } = null!;
        public bool IsActive { get; init; }
    }
    public class CreateBarangayDto
    {
        public string BarangayName { get; init; } = null!;
        public int LGUId { get; init; }
    }

    public class UpdateBarangayDto
    {
        public string BarangayName { get; init; } = null!;
        public int LGUId { get; init; }
    }        
}
