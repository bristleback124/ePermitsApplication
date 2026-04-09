using ePermitsApp.Entities.Common;

namespace ePermitsApp.Entities.BuildingPermit
{
    public class BuildingPermitSupportingDoc : BaseEntity
    {
        public int Id { get; set; }
        public int BuildingPermitId { get; set; }

        public string? SupportDocZoningClearance { get; set; }
        public string? SupportDocLocationalClearance { get; set; }
        public string? SupportDocFireSafetyClearance { get; set; }
        public string? SupportDocHighwayClearance { get; set; }
        public string? SupportDocHeightClearance { get; set; }
        public string? SupportDocECCorCNC { get; set; }
        public string? SupportDocDENRClearance { get; set; }
        public string? SupportDocSECRegistration { get; set; }
        public string? SupportDocBoardResolution { get; set; }
        public string? SupportDocHOAClearance { get; set; }

        public BuildingPermit? BuildingPermit { get; set; }
    }
}
