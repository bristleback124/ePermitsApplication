namespace ePermitsApp.DTOs
{
    public class RequirementItemDto
    {
        public int Id { get; set; }
        public string ReqDesc { get; set; } = null!;
        public int? BuildingPermitCategoryId { get; set; }
    }

    public class RequirementCategoryHierarchyDto
    {
        public int Id { get; set; }
        public string ReqCatDesc { get; set; } = null!;
        public int? BuildingPermitCategoryId { get; set; }
        public List<RequirementItemDto> Requirements { get; set; } = new();
    }

    public class RequirementClassificationHierarchyDto
    {
        public int Id { get; set; }
        public string ReqClassDesc { get; set; } = null!;
        public int? BuildingPermitCategoryId { get; set; }
        public List<RequirementCategoryHierarchyDto> Categories { get; set; } = new();
    }
}
