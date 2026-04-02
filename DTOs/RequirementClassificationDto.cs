namespace ePermitsApp.DTOs
{
    public class RequirementClassificationDto
    {
        public int Id { get; set; }
        public string ReqClassDesc { get; set; } = null!;
        public string ApplicationTypeScope { get; set; } = "Both";
    }

    public class CreateRequirementClassificationDto
    {
        public string ReqClassDesc { get; set; } = null!;
        public string ApplicationTypeScope { get; set; } = "Both";
    }

    public class UpdateRequirementClassificationDto
    {
        public string ReqClassDesc { get; set; } = null!;
        public string ApplicationTypeScope { get; set; } = "Both";
    }
}
