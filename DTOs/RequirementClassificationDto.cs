namespace ePermitsApp.DTOs
{
    public class RequirementClassificationDto
    {
        public int Id { get; set; }
        public string ReqClassDesc { get; set; } = null!;
    }

    public class CreateRequirementClassificationDto
    {
        public string ReqClassDesc { get; set; } = null!;
    }

    public class UpdateRequirementClassificationDto
    {
        public string ReqClassDesc { get; set; } = null!;
    }
}
