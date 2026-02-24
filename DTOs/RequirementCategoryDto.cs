namespace ePermitsApp.DTOs
{
    public class RequirementCategoryDto
    {
        public int Id { get; set; }
        public string ReqCatDesc { get; set; } = null!;
        public int ReqClassId { get; set; }
        public string ReqClassDesc { get; set; } = null!;
    }

    public class CreateRequirementCategoryDto
    {
        public string ReqCatDesc { get; set; } = null!;
        public int ReqClassId { get; set; }
    }

    public class UpdateRequirementCategoryDto
    {
        public string ReqCatDesc { get; set; } = null!;
        public int ReqClassId { get; set; }
    }
}
