namespace ePermitsApp.DTOs
{
    public class OwnershipTypeDto
    {
        public int Id { get; set; }
        public string OwnershipTypeDesc { get; set; } = null!;
    }

    public class CreateOwnershipTypeDto
    {
        public string OwnershipTypeDesc { get; set; } = null!;
    }

    public class UpdateOwnershipTypeDto
    {
        public string OwnershipTypeDesc { get; set; } = null!;
    }
}
