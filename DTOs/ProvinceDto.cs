namespace ePermitsApp.DTOs
{
    public class ProvinceDto
    {
        public int Id { get; set; }
        public string ProvinceName { get; set; } = null!;
    }

    public class CreateProvinceDto
    {
        public string ProvinceName { get; set; } = null!;
    }

    public class UpdateProvinceDto
    {
        public string ProvinceName { get; set; } = null!;
    }
}
