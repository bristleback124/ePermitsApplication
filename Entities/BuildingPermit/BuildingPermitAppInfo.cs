using ePermitsApp.Entities.Common;

namespace ePermitsApp.Entities.BuildingPermit
{
    public class BuildingPermitAppInfo : BaseEntity
    {
        public int Id { get; set; }
        public int BuildingPermitId { get; set; }
        public int ApplicantTypeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TIN { get; set; } = string.Empty;
        public string MailAddress { get; set; } = string.Empty;
        public int OwnershipTypeId { get; set; }

        // Document file paths
        public string ReqDocProofOwnership { get; set; } = string.Empty;
        public string ReqDocBarangayClearance { get; set; } = string.Empty;
        public string ReqDocTaxDeclaration { get; set; } = string.Empty;
        public string ReqDocRealPropTaxReceipt { get; set; } = string.Empty;
        public string? ReqDocECCorCNC { get; set; }
        public string? ReqDocSpecialClearances { get; set; }
        
        public BuildingPermit? BuildingPermit { get; set; }
    }
}
