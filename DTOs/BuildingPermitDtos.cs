using Microsoft.AspNetCore.Http;

namespace ePermitsApp.DTOs
{
    public class BuildingPermitDto
    {
        public int Id { get; set; }
        public int PermitAppTypeId { get; set; }
        public int BuildingPermitCategoryId { get; set; }
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
        public List<string> Accessories { get; set; } = new();
        public string DigitalSignature { get; set; } = string.Empty;
        public DateTime DateofSignature { get; set; }

        public BuildingPermitAppInfoDto? AppInfo { get; set; }
        public BuildingPermitDesignProfDto? DesignProf { get; set; }
        public BuildingPermitTechDocDto? TechDoc { get; set; }
        public BuildingPermitSupportingDocDto? SupportingDoc { get; set; }
    }

    public class BuildingPermitAppInfoDto
    {
        public int Id { get; set; }
        public int ApplicantTypeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TIN { get; set; } = string.Empty;
        public string MailAddress { get; set; } = string.Empty;
        public int OwnershipTypeId { get; set; }
        public FileMetadataDto ReqDocProofOwnership { get; set; } = new();
        public FileMetadataDto ReqDocBarangayClearance { get; set; } = new();
        public FileMetadataDto ReqDocTaxDeclaration { get; set; } = new();
        public FileMetadataDto ReqDocRealPropTaxReceipt { get; set; } = new();
        public FileMetadataDto? ReqDocECCorCNC { get; set; }
        public FileMetadataDto? ReqDocSpecialClearances { get; set; }
    }

    public class BuildingPermitDesignProfDto
    {
        public int Id { get; set; }
        public string IoCFullName { get; set; } = string.Empty;
        public string IoCPRCNo { get; set; } = string.Empty;
        public string IoCPTRNo { get; set; } = string.Empty;
        public DateTime IOCValidity { get; set; }
        public string SEFullName { get; set; } = string.Empty;
        public string SEPRCNo { get; set; } = string.Empty;
        public string SEPTRNo { get; set; } = string.Empty;
        public DateTime SEValidity { get; set; }
        public string EEFullName { get; set; } = string.Empty;
        public string EEPRCNo { get; set; } = string.Empty;
        public string EEPTRNo { get; set; } = string.Empty;
        public DateTime EEValidity { get; set; }
        public string SPEFullName { get; set; } = string.Empty;
        public string SPEPRCNo { get; set; } = string.Empty;
        public string SPEPTRNo { get; set; } = string.Empty;
        public DateTime SPEValidity { get; set; }
        public string? MEFullName { get; set; }
        public string? MEPRCNo { get; set; }
        public string? MEPTRNo { get; set; }
        public DateTime? MEValidity { get; set; }
        public string? GSEFullName { get; set; }
        public string? GSEPRCNo { get; set; }
        public string? GSEPTRNo { get; set; }
        public DateTime? GSEValidity { get; set; }
        public string? ECEFullName { get; set; }
        public string? ECEPRCNo { get; set; }
        public string? ECEPTRNo { get; set; }
        public DateTime? ECEValidity { get; set; }
        public string? ContractorBusinessName { get; set; }
        public string? ContractorPCABNo { get; set; }
        public string? ContractorClassification { get; set; }
        public DateTime? ContractorValidity { get; set; }
    }

    public class FileMetadataDto
    {
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string Path { get; set; } = string.Empty;
    }

    public class BuildingPermitTechDocDto
    {
        public int Id { get; set; }
        public List<FileMetadataDto> TechDocIoCPlans { get; set; } = new();
        public List<FileMetadataDto> TechDocSEPlans { get; set; } = new();
        public List<FileMetadataDto> TechDocEEPlans { get; set; } = new();
        public List<FileMetadataDto> TechDocSPPlans { get; set; } = new();
        public List<FileMetadataDto> TechDocStructuralAnalysisDesign { get; set; } = new();
        public List<FileMetadataDto> TechDocFireSafetyPlans { get; set; } = new();
        public List<FileMetadataDto> TechDocEnvironmentalDocuments { get; set; } = new();
        public List<FileMetadataDto> TechDocSoilTestFieldDensityTest { get; set; } = new();
        public List<FileMetadataDto> TechDocBOMCost { get; set; } = new();
        public List<FileMetadataDto> TechDocSoW { get; set; } = new();
        public List<FileMetadataDto> TechDocMEPlans { get; set; } = new();
        public List<FileMetadataDto> TechDocECEPlans { get; set; } = new();
    }

    public class BuildingPermitSupportingDocDto
    {
        public int Id { get; set; }
        public FileMetadataDto? SupportDocZoningClearance { get; set; }
        public FileMetadataDto? SupportDocLocationalClearance { get; set; }
        public FileMetadataDto? SupportDocFireSafetyClearance { get; set; }
        public FileMetadataDto? SupportDocHighwayClearance { get; set; }
        public FileMetadataDto? SupportDocHeightClearance { get; set; }
        public FileMetadataDto? SupportDocECCorCNC { get; set; }
        public FileMetadataDto? SupportDocDENRClearance { get; set; }
        public FileMetadataDto? SupportDocSECRegistration { get; set; }
        public FileMetadataDto? SupportDocBoardResolution { get; set; }
        public FileMetadataDto? SupportDocHOAClearance { get; set; }
    }

    public class BuildingPermitCreateDto
    {
        public int PermitAppTypeId { get; set; }
        public int BuildingPermitCategoryId { get; set; }
        public int OccupancyNatureId { get; set; }
        public string? ProjectTitle { get; set; }
        public int ProjectClassId { get; set; }
        public decimal EstimatedCost { get; set; }
        public int NoOfStoreys { get; set; }
        public decimal FloorAreaPerStorey { get; set; }
        public decimal TotalFloorArea { get; set; }
        public decimal ProjectScopeLotArea { get; set; }
        public string? PropertyAddBlock { get; set; }
        public string? PropertyAddLot { get; set; }
        public string? PropertyAddStreet { get; set; }
        public int ProvinceId { get; set; }
        public int LGUId { get; set; }
        public int BarangayId { get; set; }
        public decimal PropertyDetailLotArea { get; set; }
        public string? TCTNo { get; set; }
        public string? TaxDeclarionNo { get; set; }
        public string? Coordinates { get; set; }
        public string[] Accessories { get; set; } = Array.Empty<string>();
        public string? DigitalSignature { get; set; }
        public DateTime? DateofSignature { get; set; }

        public BuildingPermitAppInfoCreateDto AppInfo { get; set; } = new();
        public BuildingPermitDesignProfCreateDto DesignProf { get; set; } = new();
        public BuildingPermitTechDocCreateDto TechDoc { get; set; } = new();
        public BuildingPermitSupportingDocCreateDto SupportingDoc { get; set; } = new();
    }

    public class BuildingPermitEditDto
    {
        public int ApplicationId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int PermitAppTypeId { get; set; }
        public int BuildingPermitCategoryId { get; set; }
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
        public List<string> Accessories { get; set; } = new();
        public string DigitalSignature { get; set; } = string.Empty;
        public DateTime DateofSignature { get; set; }
        public BuildingPermitAppInfoDto AppInfo { get; set; } = new();
        public BuildingPermitDesignProfDto DesignProf { get; set; } = new();
        public BuildingPermitTechDocDto TechDoc { get; set; } = new();
        public BuildingPermitSupportingDocDto SupportingDoc { get; set; } = new();
    }

    public class BuildingPermitUpdateDto
    {
        public int PermitAppTypeId { get; set; }
        public int BuildingPermitCategoryId { get; set; }
        public int OccupancyNatureId { get; set; }
        public string? ProjectTitle { get; set; }
        public int ProjectClassId { get; set; }
        public decimal EstimatedCost { get; set; }
        public int NoOfStoreys { get; set; }
        public decimal FloorAreaPerStorey { get; set; }
        public decimal TotalFloorArea { get; set; }
        public decimal ProjectScopeLotArea { get; set; }
        public string? PropertyAddBlock { get; set; }
        public string? PropertyAddLot { get; set; }
        public string? PropertyAddStreet { get; set; }
        public int ProvinceId { get; set; }
        public int LGUId { get; set; }
        public int BarangayId { get; set; }
        public decimal PropertyDetailLotArea { get; set; }
        public string? TCTNo { get; set; }
        public string? TaxDeclarionNo { get; set; }
        public string? Coordinates { get; set; }
        public string[] Accessories { get; set; } = Array.Empty<string>();
        public string? DigitalSignature { get; set; }
        public DateTime? DateofSignature { get; set; }

        public BuildingPermitAppInfoUpdateDto AppInfo { get; set; } = new();
        public BuildingPermitDesignProfUpdateDto DesignProf { get; set; } = new();
        public BuildingPermitTechDocUpdateDto TechDoc { get; set; } = new();
        public BuildingPermitSupportingDocUpdateDto SupportingDoc { get; set; } = new();
    }

    public class BuildingPermitAppInfoCreateDto
    {
        public int ApplicantTypeId { get; set; }
        public string? FullName { get; set; }
        public string? ContactNo { get; set; }
        public string? Email { get; set; }
        public string? TIN { get; set; }
        public string? MailAddress { get; set; }
        public int OwnershipTypeId { get; set; }

        public IFormFile? ReqDocProofOwnership { get; set; }
        public IFormFile? ReqDocBarangayClearance { get; set; }
        public IFormFile? ReqDocTaxDeclaration { get; set; }
        public IFormFile? ReqDocRealPropTaxReceipt { get; set; }
        public IFormFile? ReqDocECCorCNC { get; set; }
        public IFormFile? ReqDocSpecialClearances { get; set; }
    }

    public class BuildingPermitAppInfoUpdateDto
    {
        public int ApplicantTypeId { get; set; }
        public string? FullName { get; set; }
        public string? ContactNo { get; set; }
        public string? Email { get; set; }
        public string? TIN { get; set; }
        public string? MailAddress { get; set; }
        public int OwnershipTypeId { get; set; }

        public bool KeepReqDocProofOwnership { get; set; }
        public bool KeepReqDocBarangayClearance { get; set; }
        public bool KeepReqDocTaxDeclaration { get; set; }
        public bool KeepReqDocRealPropTaxReceipt { get; set; }
        public bool KeepReqDocECCorCNC { get; set; }
        public bool KeepReqDocSpecialClearances { get; set; }

        public IFormFile? ReqDocProofOwnership { get; set; }
        public IFormFile? ReqDocBarangayClearance { get; set; }
        public IFormFile? ReqDocTaxDeclaration { get; set; }
        public IFormFile? ReqDocRealPropTaxReceipt { get; set; }
        public IFormFile? ReqDocECCorCNC { get; set; }
        public IFormFile? ReqDocSpecialClearances { get; set; }
    }

    public class BuildingPermitDesignProfCreateDto
    {
        public string? IoCFullName { get; set; }
        public string? IoCPRCNo { get; set; }
        public string? IoCPTRNo { get; set; }
        public DateTime? IOCValidity { get; set; }
        public string? SEFullName { get; set; }
        public string? SEPRCNo { get; set; }
        public string? SEPTRNo { get; set; }
        public DateTime? SEValidity { get; set; }
        public string? EEFullName { get; set; }
        public string? EEPRCNo { get; set; }
        public string? EEPTRNo { get; set; }
        public DateTime? EEValidity { get; set; }
        public string? SPEFullName { get; set; }
        public string? SPEPRCNo { get; set; }
        public string? SPEPTRNo { get; set; }
        public DateTime? SPEValidity { get; set; }
        public string? MEFullName { get; set; }
        public string? MEPRCNo { get; set; }
        public string? MEPTRNo { get; set; }
        public DateTime? MEValidity { get; set; }
        public string? GSEFullName { get; set; }
        public string? GSEPRCNo { get; set; }
        public string? GSEPTRNo { get; set; }
        public DateTime? GSEValidity { get; set; }
        public string? ECEFullName { get; set; }
        public string? ECEPRCNo { get; set; }
        public string? ECEPTRNo { get; set; }
        public DateTime? ECEValidity { get; set; }
        public string? ContractorBusinessName { get; set; }
        public string? ContractorPCABNo { get; set; }
        public string? ContractorClassification { get; set; }
        public DateTime? ContractorValidity { get; set; }
    }

    public class BuildingPermitDesignProfUpdateDto : BuildingPermitDesignProfCreateDto
    {
    }

    public class BuildingPermitTechDocCreateDto
    {
        public IFormFileCollection? TechDocIoCPlans { get; set; }
        public IFormFileCollection? TechDocSEPlans { get; set; }
        public IFormFileCollection? TechDocEEPlans { get; set; }
        public IFormFileCollection? TechDocSPPlans { get; set; }
        public IFormFileCollection? TechDocStructuralAnalysisDesign { get; set; }
        public IFormFileCollection? TechDocFireSafetyPlans { get; set; }
        public IFormFileCollection? TechDocEnvironmentalDocuments { get; set; }
        public IFormFileCollection? TechDocSoilTestFieldDensityTest { get; set; }
        public IFormFileCollection? TechDocBOMCost { get; set; }
        public IFormFileCollection? TechDocSoW { get; set; }
        public IFormFileCollection? TechDocMEPlans { get; set; }
        public IFormFileCollection? TechDocECEPlans { get; set; }
    }

    public class BuildingPermitTechDocUpdateDto
    {
        public string[] KeepTechDocIoCPlans { get; set; } = Array.Empty<string>();
        public string[] KeepTechDocSEPlans { get; set; } = Array.Empty<string>();
        public string[] KeepTechDocEEPlans { get; set; } = Array.Empty<string>();
        public string[] KeepTechDocSPPlans { get; set; } = Array.Empty<string>();
        public string[] KeepTechDocStructuralAnalysisDesign { get; set; } = Array.Empty<string>();
        public string[] KeepTechDocFireSafetyPlans { get; set; } = Array.Empty<string>();
        public string[] KeepTechDocEnvironmentalDocuments { get; set; } = Array.Empty<string>();
        public string[] KeepTechDocSoilTestFieldDensityTest { get; set; } = Array.Empty<string>();
        public string[] KeepTechDocBOMCost { get; set; } = Array.Empty<string>();
        public string[] KeepTechDocSoW { get; set; } = Array.Empty<string>();
        public string[] KeepTechDocMEPlans { get; set; } = Array.Empty<string>();
        public string[] KeepTechDocECEPlans { get; set; } = Array.Empty<string>();

        public IFormFileCollection? TechDocIoCPlans { get; set; }
        public IFormFileCollection? TechDocSEPlans { get; set; }
        public IFormFileCollection? TechDocEEPlans { get; set; }
        public IFormFileCollection? TechDocSPPlans { get; set; }
        public IFormFileCollection? TechDocStructuralAnalysisDesign { get; set; }
        public IFormFileCollection? TechDocFireSafetyPlans { get; set; }
        public IFormFileCollection? TechDocEnvironmentalDocuments { get; set; }
        public IFormFileCollection? TechDocSoilTestFieldDensityTest { get; set; }
        public IFormFileCollection? TechDocBOMCost { get; set; }
        public IFormFileCollection? TechDocSoW { get; set; }
        public IFormFileCollection? TechDocMEPlans { get; set; }
        public IFormFileCollection? TechDocECEPlans { get; set; }
    }

    public class BuildingPermitSupportingDocCreateDto
    {
        public IFormFile? SupportDocZoningClearance { get; set; }
        public IFormFile? SupportDocLocationalClearance { get; set; }
        public IFormFile? SupportDocFireSafetyClearance { get; set; }
        public IFormFile? SupportDocHighwayClearance { get; set; }
        public IFormFile? SupportDocHeightClearance { get; set; }
        public IFormFile? SupportDocECCorCNC { get; set; }
        public IFormFile? SupportDocDENRClearance { get; set; }
        public IFormFile? SupportDocSECRegistration { get; set; }
        public IFormFile? SupportDocBoardResolution { get; set; }
        public IFormFile? SupportDocHOAClearance { get; set; }
    }

    public class BuildingPermitSupportingDocUpdateDto
    {
        public bool KeepSupportDocZoningClearance { get; set; }
        public bool KeepSupportDocLocationalClearance { get; set; }
        public bool KeepSupportDocFireSafetyClearance { get; set; }
        public bool KeepSupportDocHighwayClearance { get; set; }
        public bool KeepSupportDocHeightClearance { get; set; }
        public bool KeepSupportDocECCorCNC { get; set; }
        public bool KeepSupportDocDENRClearance { get; set; }
        public bool KeepSupportDocSECRegistration { get; set; }
        public bool KeepSupportDocBoardResolution { get; set; }
        public bool KeepSupportDocHOAClearance { get; set; }

        public IFormFile? SupportDocZoningClearance { get; set; }
        public IFormFile? SupportDocLocationalClearance { get; set; }
        public IFormFile? SupportDocFireSafetyClearance { get; set; }
        public IFormFile? SupportDocHighwayClearance { get; set; }
        public IFormFile? SupportDocHeightClearance { get; set; }
        public IFormFile? SupportDocECCorCNC { get; set; }
        public IFormFile? SupportDocDENRClearance { get; set; }
        public IFormFile? SupportDocSECRegistration { get; set; }
        public IFormFile? SupportDocBoardResolution { get; set; }
        public IFormFile? SupportDocHOAClearance { get; set; }
    }
}
