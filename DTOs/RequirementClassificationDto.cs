namespace ePermitsApp.DTOs
{
    public class RequirementClassificationDto
    {
        public int Id { get; set; }
        public string ReqClassDesc { get; set; } = null!;
        public string ApplicationTypeScope { get; set; } = "Both";
        public int? BuildingPermitCategoryId { get; set; }
    }

    public class CreateRequirementClassificationDto
    {
        public string ReqClassDesc { get; set; } = null!;
        public string ApplicationTypeScope { get; set; } = "Both";
        public int? BuildingPermitCategoryId { get; set; }
    }

    public class UpdateRequirementClassificationDto
    {
        public string ReqClassDesc { get; set; } = null!;
        public string ApplicationTypeScope { get; set; } = "Both";
        public int? BuildingPermitCategoryId { get; set; }
    }
}
