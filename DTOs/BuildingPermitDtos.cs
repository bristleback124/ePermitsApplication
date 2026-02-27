using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ePermitsApp.DTOs
{
    public class BuildingPermitDto
    {
        public int Id { get; set; }
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

        public BuildingPermitAppInfoDto? AppInfo { get; set; }
        public BuildingPermitDesignProfDto? DesignProf { get; set; }
        public BuildingPermitTechDocDto? TechDoc { get; set; }
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
        public List<FileMetadataDto> TechDocBOMCost { get; set; } = new();
        public List<FileMetadataDto> TechDocSoW { get; set; } = new();
        public List<FileMetadataDto> TechDocMEPlans { get; set; } = new();
        public List<FileMetadataDto> TechDocECEPlans { get; set; } = new();
    }

    public class BuildingPermitCreateDto
    {
        [Required]
        public int PermitAppTypeId { get; set; }
        [Required]
        public int OccupancyNatureId { get; set; }
        [Required]
        public string ProjectTitle { get; set; } = string.Empty;
        [Required]
        public int ProjectClassId { get; set; }
        [Required]
        public decimal EstimatedCost { get; set; }
        [Required]
        public int NoOfStoreys { get; set; }
        [Required]
        public decimal FloorAreaPerStorey { get; set; }
        [Required]
        public decimal TotalFloorArea { get; set; }
        [Required]
        public decimal ProjectScopeLotArea { get; set; }
        public string? PropertyAddBlock { get; set; }
        [Required]
        public string PropertyAddLot { get; set; } = string.Empty;
        [Required]
        public string PropertyAddStreet { get; set; } = string.Empty;
        [Required]
        public int ProvinceId { get; set; }
        [Required]
        public int LGUId { get; set; }
        [Required]
        public int BarangayId { get; set; }
        [Required]
        public decimal PropertyDetailLotArea { get; set; }
        [Required]
        public string TCTNo { get; set; } = string.Empty;
        [Required]
        public string TaxDeclarionNo { get; set; } = string.Empty;
        public string? Coordinates { get; set; }
        [Required]
        public string DigitalSignature { get; set; } = string.Empty;
        [Required]
        public DateTime DateofSignature { get; set; }

        public BuildingPermitAppInfoCreateDto AppInfo { get; set; } = null!;
        public BuildingPermitDesignProfCreateDto DesignProf { get; set; } = null!;
        public BuildingPermitTechDocCreateDto TechDoc { get; set; } = null!;
    }

    public class BuildingPermitAppInfoCreateDto
    {
        [Required]
        public int ApplicantTypeId { get; set; }
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string ContactNo { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string TIN { get; set; } = string.Empty;
        [Required]
        public string MailAddress { get; set; } = string.Empty;
        [Required]
        public int OwnershipTypeId { get; set; }

        [Required]
        public IFormFile ReqDocProofOwnership { get; set; } = null!;
        [Required]
        public IFormFile ReqDocBarangayClearance { get; set; } = null!;
        [Required]
        public IFormFile ReqDocTaxDeclaration { get; set; } = null!;
        [Required]
        public IFormFile ReqDocRealPropTaxReceipt { get; set; } = null!;
        public IFormFile? ReqDocECCorCNC { get; set; }
        public IFormFile? ReqDocSpecialClearances { get; set; }
    }

    public class BuildingPermitDesignProfCreateDto
    {
        [Required]
        public string IoCFullName { get; set; } = string.Empty;
        [Required]
        public string IoCPRCNo { get; set; } = string.Empty;
        [Required]
        public string IoCPTRNo { get; set; } = string.Empty;
        [Required]
        public DateTime IOCValidity { get; set; }
        [Required]
        public string SEFullName { get; set; } = string.Empty;
        [Required]
        public string SEPRCNo { get; set; } = string.Empty;
        [Required]
        public string SEPTRNo { get; set; } = string.Empty;
        [Required]
        public DateTime SEValidity { get; set; }
        [Required]
        public string EEFullName { get; set; } = string.Empty;
        [Required]
        public string EEPRCNo { get; set; } = string.Empty;
        [Required]
        public string EEPTRNo { get; set; } = string.Empty;
        [Required]
        public DateTime EEValidity { get; set; }
        [Required]
        public string SPEFullName { get; set; } = string.Empty;
        [Required]
        public string SPEPRCNo { get; set; } = string.Empty;
        [Required]
        public string SPEPTRNo { get; set; } = string.Empty;
        [Required]
        public DateTime SPEValidity { get; set; }
        public string? MEFullName { get; set; }
        public string? MEPRCNo { get; set; }
        public string? MEPTRNo { get; set; }
        public DateTime? MEValidity { get; set; }
        public string? ECEFullName { get; set; }
        public string? ECEPRCNo { get; set; }
        public string? ECEPTRNo { get; set; }
        public DateTime? ECEValidity { get; set; }
        public string? ContractorBusinessName { get; set; }
        public string? ContractorPCABNo { get; set; }
        public string? ContractorClassification { get; set; }
        public DateTime? ContractorValidity { get; set; }
    }

    public class BuildingPermitTechDocCreateDto
    {
        [Required]
        public IFormFileCollection TechDocIoCPlans { get; set; } = null!;
        [Required]
        public IFormFileCollection TechDocSEPlans { get; set; } = null!;
        [Required]
        public IFormFileCollection TechDocEEPlans { get; set; } = null!;
        [Required]
        public IFormFileCollection TechDocSPPlans { get; set; } = null!;
        [Required]
        public IFormFileCollection TechDocBOMCost { get; set; } = null!;
        [Required]
        public IFormFileCollection TechDocSoW { get; set; } = null!;
        public IFormFileCollection? TechDocMEPlans { get; set; }
        public IFormFileCollection? TechDocECEPlans { get; set; }
    }
}
