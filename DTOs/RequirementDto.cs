namespace ePermitsApp.DTOs
{
    public class RequirementDto
    {
        public int Id { get; init; }
        public string ReqDesc { get; init; } = null!;
        public int ReqCatId { get; init; }
        public string ReqCatDesc { get; init; } = null!;
        public string ReqClassDesc { get; init; } = null!;
        public string ApplicationTypeScope { get; init; } = "Both";
    }
    public class CreateRequirementDto
    {
        public string ReqDesc { get; init; } = null!;
        public int ReqCatId { get; init; }
        public string ApplicationTypeScope { get; init; } = "Both";
    }

    public class UpdateRequirementDto
    {
        public string ReqDesc { get; init; } = null!;
        public int ReqCatId { get; init; }
        public string ApplicationTypeScope { get; init; } = "Both";
    }
}
