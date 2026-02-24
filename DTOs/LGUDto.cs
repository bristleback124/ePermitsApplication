namespace ePermitsApp.DTOs
{
    public class LGUDto
    {
        public int Id { get; set; }
        public string LGUName { get; set; } = null!;
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; } = null!;
    }

    public class CreateLGUDto
    {
        public string LGUName { get; set; } = null!;
        public int ProvinceId { get; set; }
    }

    public class UpdateLGUDto
    {
        public string LGUName { get; set; } = null!;
        public int ProvinceId { get; set; }
    }
}
