namespace ePermitsApp.DTOs
{
    public class PermitApplicationTypeDto
    {
        public int Id { get; set; }
        public string PermitAppTypeDesc { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreatePermitApplicationTypeDto
    {
        public string PermitAppTypeDesc { get; set; } = null!;
    }

    public class UpdatePermitApplicationTypeDto
    {
        public string PermitAppTypeDesc { get; set; } = null!;
    }
}
