namespace ePermitsApp.DTOs
{
    public class OccupancyNatureDto
    {
        public int Id { get; set; }
        public string OccupancyNatureDesc { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateOccupancyNatureDto
    {
        public string OccupancyNatureDesc { get; set; } = null!;
    }

    public class UpdateOccupancyNatureDto
    {
        public string OccupancyNatureDesc { get; set; } = null!;
    }
}
