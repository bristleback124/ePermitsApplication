using System;
using System.Collections.Generic;

namespace ePermitsApp.DTOs
{
    public class ApplicationDtoShort
    {
        public int Id { get; set; }
        public string FormattedId { get; set; } = string.Empty;
        public string ProjectTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ApplicationDepartmentReviewDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentCode { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? AssignedReviewerId { get; set; }
        public string? AssignedReviewerName { get; set; }
        public DateTime? AssignedAt { get; set; }
    }

    public class ReviewerDashboardItemDto
    {
        public int Id { get; set; }
        public string FormattedId { get; set; } = string.Empty;
        public string ProjectTitle { get; set; } = string.Empty;
        public string Applicant { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastActionAt { get; set; }
        public List<ApplicationDepartmentReviewDto> ReviewOffices { get; set; } = new();
    }

    public class ApplicationBuildingPermitDetailDto
    {
        public BasicInformationDto BasicInformation { get; set; } = new();
        public ProjectDetailsDto ProjectDetails { get; set; } = new();
        public OwnerInformationDto OwnerInformation { get; set; } = new();
        public RequiredDocs RequiredDocs { get; set; } = new();
        public BuildingPermitTechDocDto BuildingPermitTechDocs { get; set; } = new();
        public BuildingPermitSupportingDocDto SupportingDocs { get; set; } = new();
        public List<ApplicationDepartmentReviewDto> ReviewOffices { get; set; } = new();
    }

    public class RequiredDocs
    {
        public FileMetadataDto ReqDocProofOwnership { get; set; } = new();
        public FileMetadataDto ReqDocBarangayClearance { get; set; } = new();
        public FileMetadataDto ReqDocTaxDeclaration { get; set; } = new();
        public FileMetadataDto ReqDocRealPropTaxReceipt { get; set; } = new();
        public FileMetadataDto? ReqDocECCorCNC { get; set; }
        public FileMetadataDto? ReqDocSpecialClearances { get; set; }
    }

    public class BasicInformationDto
    {
        public int ApplicationId { get; set; }
        public string FormattedId { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? StatusReason { get; set; }
        public string? NextStep { get; set; }
        public string Applicant { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class ProjectDetailsDto
    {
        public string ProjectType { get; set; } = string.Empty;
        public decimal ProjectValue { get; set; }
        public decimal LotArea { get; set; }
        public int NumberOfStories { get; set; }
        public string PermitType { get; set; } = string.Empty;
        public string OccupancyType { get; set; } = string.Empty;
        public decimal FloorArea { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class OwnerInformationDto
    {
        public string OwnerName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ApplicationCoODetailDto
    {
        public BasicInformationDto BasicInformation { get; set; } = new();
        public CoOProjectDetailsDto ProjectDetails { get; set; } = new();
        public OwnerInformationDto OwnerInformation { get; set; } = new();
        public CoORequiredDocsDto RequiredDocs { get; set; } = new();
        public CoOProfessionalInfoDto ProfessionalInfo { get; set; } = new();
        public List<ApplicationDepartmentReviewDto> ReviewOffices { get; set; } = new();
    }

    public class CoOProjectDetailsDto
    {
        public string ProjectType { get; set; } = string.Empty;
        public string BldgPermitNo { get; set; } = string.Empty;
        public string OccupancyType { get; set; } = string.Empty;
        public decimal FloorArea { get; set; }
        public int NumberOfStories { get; set; }
        public DateTime CompletionDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CoORequiredDocsDto
    {
        public FileMetadataDto ReqDocBldgPermitSPlans { get; set; } = new();
        public FileMetadataDto ReqDocAsBuiltPlans { get; set; } = new();
        public FileMetadataDto ReqDocConsLogbook { get; set; } = new();
        public FileMetadataDto ReqDocConsPhotos { get; set; } = new();
        public FileMetadataDto ReqDocBrgyClearance { get; set; } = new();
        public FileMetadataDto ReqDocFSIC { get; set; } = new();
        public FileMetadataDto? ReqDocOthers { get; set; }
    }

    public class CoOProfessionalInfoDto
    {
        public string IoCFullName { get; set; } = string.Empty;
        public string IoCPRCNo { get; set; } = string.Empty;
        public string IoCPTRNo { get; set; } = string.Empty;
        public DateTime IOCValidity { get; set; }
        public string EoRFullName { get; set; } = string.Empty;
        public string EoRPRCorPTRNo { get; set; } = string.Empty;
        public DateTime EoRValidity { get; set; }
        public string EoRSpecialization { get; set; } = string.Empty;
    }
}
