using System;
using System.Collections.Generic;

namespace ePermitsApp.DTOs
{
    public class ApplicationDtoShort
    {
        public int Id { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ApplicationBuildingPermitDetailDto
    {
        public BasicInformationDto BasicInformation { get; set; } = new();
        public ProjectDetailsDto ProjectDetails { get; set; } = new();
        public OwnerInformationDto OwnerInformation { get; set; } = new();
        public RequiredDocs RequiredDocs { get; set; } = new();
        public BuildingPermitTechDocDto BuildingPermitTechDocs { get; set; } = new();
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
        public string ProjectDescription { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
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
}
