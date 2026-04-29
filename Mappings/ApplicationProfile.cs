using AutoMapper;
using ePermits.Models;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Entities.BuildingPermit;
using ePermitsApp.Entities.CoOApp;
using ePermitsApp.Helpers;

namespace ePermitsApp.Mappings
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Application, ApplicationDtoShort>()
                .ForMember(dest => dest.FormattedId, opt => opt.MapFrom(src => src.FormattedId))
                .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src =>
                    src.BuildingPermit != null ? src.BuildingPermit.ProjectTitle
                    : src.CoOApp != null ? src.CoOApp.ProjectTitle
                    : string.Empty))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

            CreateMap<ApplicationDepartmentReview, ApplicationDepartmentReviewDto>()
                .ForMember(dest => dest.DepartmentCode, opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentCode : string.Empty))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName : string.Empty))
                .ForMember(dest => dest.AssignedReviewerName, opt => opt.MapFrom(src =>
                    src.AssignedReviewer != null && src.AssignedReviewer.UserProfile != null
                        ? $"{src.AssignedReviewer.UserProfile.FirstName} {src.AssignedReviewer.UserProfile.LastName}".Trim()
                        : src.AssignedReviewer != null ? src.AssignedReviewer.Username : null));

            CreateMap<Application, ReviewerDashboardItemDto>()
                .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src =>
                    src.BuildingPermit != null ? src.BuildingPermit.ProjectTitle
                    : src.CoOApp != null ? src.CoOApp.ProjectTitle
                    : string.Empty))
                .ForMember(dest => dest.Applicant, opt => opt.MapFrom(src => src.User != null && src.User.UserProfile != null
                    ? $"{src.User.UserProfile.FirstName} {src.User.UserProfile.LastName}".Trim()
                    : string.Empty))
                .ForMember(dest => dest.BuildingPermitCategoryName, opt => opt.MapFrom(src =>
                    src.BuildingPermit != null && src.BuildingPermit.BuildingPermitCategory != null
                        ? src.BuildingPermit.BuildingPermitCategory.CategoryName
                        : null))
                .ForMember(dest => dest.LastActionAt, opt => opt.MapFrom(src => src.UpdatedAt ?? src.CreatedAt))
                .ForMember(dest => dest.ReviewOffices, opt => opt.MapFrom(src => src.DepartmentReviews));

            CreateMap<Application, ApplicationBuildingPermitDetailDto>()
                .ForMember(dest => dest.BasicInformation, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.ProjectDetails, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.OwnerInformation, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.RequiredDocs, opt => opt.MapFrom(src => src.BuildingPermit != null ? src.BuildingPermit.AppInfo : null))
                .ForMember(dest => dest.BuildingPermitTechDocs, opt => opt.MapFrom(src => src.BuildingPermit != null ? src.BuildingPermit.TechDoc : null))
                .ForMember(dest => dest.SupportingDocs, opt => opt.MapFrom(src => src.BuildingPermit != null ? src.BuildingPermit.SupportingDoc : null))
                .ForMember(dest => dest.ReviewOffices, opt => opt.MapFrom(src => src.DepartmentReviews));

            // Mapping for BuildingPermitTechDoc
            CreateMap<BuildingPermitTechDoc, BuildingPermitTechDocDto>()
                .ForMember(dest => dest.TechDocIoCPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocIoCPlans)))
                .ForMember(dest => dest.TechDocSEPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocSEPlans)))
                .ForMember(dest => dest.TechDocEEPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocEEPlans)))
                .ForMember(dest => dest.TechDocSPPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocSPPlans)))
                .ForMember(dest => dest.TechDocStructuralAnalysisDesign, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocStructuralAnalysisDesign)))
                .ForMember(dest => dest.TechDocFireSafetyPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocFireSafetyPlans)))
                .ForMember(dest => dest.TechDocEnvironmentalDocuments, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocEnvironmentalDocuments)))
                .ForMember(dest => dest.TechDocSoilTestFieldDensityTest, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocSoilTestFieldDensityTest)))
                .ForMember(dest => dest.TechDocBOMCost, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocBOMCost)))
                .ForMember(dest => dest.TechDocSoW, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocSoW)))
                .ForMember(dest => dest.TechDocMEPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocMEPlans)))
                .ForMember(dest => dest.TechDocECEPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocECEPlans)));

            CreateMap<BuildingPermitSupportingDoc, BuildingPermitSupportingDocDto>()
                .ForMember(dest => dest.SupportDocZoningClearance, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.SupportDocZoningClearance)))
                .ForMember(dest => dest.SupportDocLocationalClearance, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.SupportDocLocationalClearance)))
                .ForMember(dest => dest.SupportDocFireSafetyClearance, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.SupportDocFireSafetyClearance)))
                .ForMember(dest => dest.SupportDocHighwayClearance, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.SupportDocHighwayClearance)))
                .ForMember(dest => dest.SupportDocHeightClearance, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.SupportDocHeightClearance)))
                .ForMember(dest => dest.SupportDocECCorCNC, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.SupportDocECCorCNC)))
                .ForMember(dest => dest.SupportDocDENRClearance, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.SupportDocDENRClearance)))
                .ForMember(dest => dest.SupportDocSECRegistration, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.SupportDocSECRegistration)))
                .ForMember(dest => dest.SupportDocBoardResolution, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.SupportDocBoardResolution)))
                .ForMember(dest => dest.SupportDocHOAClearance, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.SupportDocHOAClearance)));

            // Mapping for RequiredDocs
            CreateMap<BuildingPermitAppInfo, RequiredDocs>()
                .ForMember(dest => dest.ReqDocProofOwnership, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocProofOwnership)))
                .ForMember(dest => dest.ReqDocBarangayClearance, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocBarangayClearance)))
                .ForMember(dest => dest.ReqDocTaxDeclaration, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocTaxDeclaration)))
                .ForMember(dest => dest.ReqDocRealPropTaxReceipt, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocRealPropTaxReceipt)))
                .ForMember(dest => dest.ReqDocECCorCNC, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocECCorCNC)))
                .ForMember(dest => dest.ReqDocSpecialClearances, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocSpecialClearances)));

            // Mapping for BasicInformationDto
            CreateMap<Application, BasicInformationDto>()
                .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FormattedId, opt => opt.MapFrom(src => src.FormattedId))
                .ForMember(dest => dest.ProjectDescription, opt => opt.MapFrom(src =>
                    src.BuildingPermit != null ? src.BuildingPermit.ProjectTitle
                    : src.CoOApp != null ? src.CoOApp.ProjectTitle
                    : string.Empty))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.StatusReason, opt => opt.MapFrom(src => src.StatusReason))
                .ForMember(dest => dest.RequirementsReviewStatus, opt => opt.MapFrom(src => src.RequirementsReviewStatus))
                .ForMember(dest => dest.TechnicalPlansReviewStatus, opt => opt.MapFrom(src => src.TechnicalPlansReviewStatus))
                .ForMember(dest => dest.NextStep, opt => opt.MapFrom(src =>
                    ApplicationWorkflowDefinitions.NextStepDescriptions.ContainsKey(src.Status)
                        ? ApplicationWorkflowDefinitions.NextStepDescriptions[src.Status] : null))
                .ForMember(dest => dest.Applicant, opt => opt.MapFrom(src => src.User != null && src.User.UserProfile != null
                    ? $"{src.User.UserProfile.FirstName} {src.User.UserProfile.LastName}" : string.Empty))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User != null && src.User.UserProfile != null
                    ? src.User.UserProfile.MobileNo : string.Empty));

            // Mapping for ProjectDetailsDto
            CreateMap<Application, ProjectDetailsDto>()
                .ForMember(dest => dest.ProjectType, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.ProjectValue, opt => opt.MapFrom(src => src.BuildingPermit != null ? src.BuildingPermit.EstimatedCost : 0))
                .ForMember(dest => dest.LotArea, opt => opt.MapFrom(src => src.BuildingPermit != null ? src.BuildingPermit.PropertyDetailLotArea : 0))
                .ForMember(dest => dest.NumberOfStories, opt => opt.MapFrom(src => src.BuildingPermit != null ? src.BuildingPermit.NoOfStoreys : 0))
                .ForMember(dest => dest.PermitType, opt => opt.MapFrom(src => src.BuildingPermit != null && src.BuildingPermit.PermitApplicationType != null 
                    ? src.BuildingPermit.PermitApplicationType.PermitAppTypeDesc : string.Empty))
                .ForMember(dest => dest.OccupancyType, opt => opt.MapFrom(src => src.BuildingPermit != null && src.BuildingPermit.OccupancyNature != null 
                    ? src.BuildingPermit.OccupancyNature.OccupancyNatureDesc : string.Empty))
                .ForMember(dest => dest.FloorArea, opt => opt.MapFrom(src => src.BuildingPermit != null ? src.BuildingPermit.TotalFloorArea : 0))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // Mapping for OwnerInformationDto
            CreateMap<Application, OwnerInformationDto>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src =>
                    src.BuildingPermit != null && src.BuildingPermit.AppInfo != null ? src.BuildingPermit.AppInfo.FullName
                    : src.CoOApp != null ? src.CoOApp.FullName
                    : string.Empty))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src =>
                    src.BuildingPermit != null && src.BuildingPermit.AppInfo != null ? src.BuildingPermit.AppInfo.MailAddress
                    : src.CoOApp != null ? src.CoOApp.MailAddress
                    : string.Empty))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src =>
                    src.BuildingPermit != null && src.BuildingPermit.AppInfo != null ? src.BuildingPermit.AppInfo.ContactNo
                    : src.CoOApp != null ? src.CoOApp.ContactNo
                    : string.Empty))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src =>
                    src.BuildingPermit != null && src.BuildingPermit.AppInfo != null ? src.BuildingPermit.AppInfo.Email
                    : src.CoOApp != null ? src.CoOApp.Email
                    : string.Empty));

            // CoO Detail mappings
            CreateMap<Application, ApplicationCoODetailDto>()
                .ForMember(dest => dest.BasicInformation, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.ProjectDetails, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.OwnerInformation, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.RequiredDocs, opt => opt.MapFrom(src => src.CoOApp != null ? src.CoOApp.CoOAppReqDoc : null))
                .ForMember(dest => dest.ProfessionalInfo, opt => opt.MapFrom(src => src.CoOApp != null ? src.CoOApp.CoOAppProf : null))
                .ForMember(dest => dest.ReviewOffices, opt => opt.MapFrom(src => src.DepartmentReviews));

            CreateMap<Application, CoOProjectDetailsDto>()
                .ForMember(dest => dest.ProjectType, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.BldgPermitNo, opt => opt.MapFrom(src => src.CoOApp != null ? src.CoOApp.BldgPermitNo : string.Empty))
                .ForMember(dest => dest.OccupancyType, opt => opt.MapFrom(src => src.CoOApp != null && src.CoOApp.OccupancyNature != null
                    ? src.CoOApp.OccupancyNature.OccupancyNatureDesc : string.Empty))
                .ForMember(dest => dest.FloorArea, opt => opt.MapFrom(src => src.CoOApp != null ? src.CoOApp.FloorArea : 0))
                .ForMember(dest => dest.NumberOfStories, opt => opt.MapFrom(src => src.CoOApp != null ? src.CoOApp.NoOfStoreys : 0))
                .ForMember(dest => dest.CompletionDate, opt => opt.MapFrom(src => src.CoOApp != null ? src.CoOApp.CompletionDate : DateTime.MinValue))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<CoOAppReqDoc, CoORequiredDocsDto>()
                .ForMember(dest => dest.ReqDocBldgPermitSPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocBldgPermitSPlans)))
                .ForMember(dest => dest.ReqDocAsBuiltPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocAsBuiltPlans)))
                .ForMember(dest => dest.ReqDocConsLogbook, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocConsLogbook)))
                .ForMember(dest => dest.ReqDocConsPhotos, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocConsPhotos)))
                .ForMember(dest => dest.ReqDocBrgyClearance, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocBrgyClearance)))
                .ForMember(dest => dest.ReqDocFSIC, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocFSIC)))
                .ForMember(dest => dest.ReqDocOthers, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocOthers)));

            CreateMap<CoOAppProf, CoOProfessionalInfoDto>();
        }
    }
}
