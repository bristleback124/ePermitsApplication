using ePermitsApp.Entities.Common;

namespace ePermitsApp.Entities.BuildingPermit
{
    public class BuildingPermitTechDoc : BaseEntity
    {
        public int Id { get; set; }
        public int BuildingPermitId { get; set; }

        // Document file paths (required)
        public string TechDocIoCPlans { get; set; } = string.Empty;
        public string TechDocSEPlans { get; set; } = string.Empty;
        public string TechDocEEPlans { get; set; } = string.Empty;
        public string TechDocSPPlans { get; set; } = string.Empty;
        public string TechDocBOMCost { get; set; } = string.Empty;
        public string TechDocSoW { get; set; } = string.Empty;

        // Document file paths (optional)
        public string? TechDocMEPlans { get; set; }
        public string? TechDocECEPlans { get; set; }

        // Navigation Properties
        public BuildingPermit? BuildingPermit { get; set; }
    }
}
