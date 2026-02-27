using ePermitsApp.Entities.Common;
using ePermits.Models;

namespace ePermitsApp.Entities.BuildingPermit
{
    public class BuildingPermit : BaseEntity
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int PermitAppTypeId { get; set; }
        public int OccupancyNatureId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public int ProjectClassId { get; set; }
        public decimal EstimatedCost { get; set; }
        public int NoOfStoreys { get; set; }
        public decimal FloorAreaPerStorey { get; set; }
        public decimal TotalFloorArea { get; set; }
        public decimal ProjectScopeLotArea { get; set; }
        public string? PropertyAddBlock { get; set; }
        public string PropertyAddLot { get; set; } = string.Empty;
        public string PropertyAddStreet { get; set; } = string.Empty;
        public int ProvinceId { get; set; }
        public int LGUId { get; set; }
        public int BarangayId { get; set; }
        public decimal PropertyDetailLotArea { get; set; }
        public string TCTNo { get; set; } = string.Empty;
        public string TaxDeclarionNo { get; set; } = string.Empty;
        public string? Coordinates { get; set; }
        public string DigitalSignature { get; set; } = string.Empty;
        public DateTime DateofSignature { get; set; }
        
        public Application? Application { get; set; }
        public BuildingPermitAppInfo? AppInfo { get; set; }
        public BuildingPermitDesignProf? DesignProf { get; set; }
        public BuildingPermitTechDoc? TechDoc { get; set; }

        // Navigation Properties for lookups
        public PermitApplicationType? PermitApplicationType { get; set; }
        public OccupancyNature? OccupancyNature { get; set; }
        public ProjectClassification? ProjectClassification { get; set; }
        public Province? Province { get; set; }
        public LGU? LGU { get; set; }
        public Barangay? Barangay { get; set; }
    }
}
