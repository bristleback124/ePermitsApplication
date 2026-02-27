using AutoMapper;
using ePermits.Models;
using ePermitsApp.DTOs;
using ePermitsApp.Entities.BuildingPermit;
using ePermitsApp.Helpers;

namespace ePermitsApp.Mappings
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Application, ApplicationDtoShort>()
                .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src => src.BuildingPermit != null ? src.BuildingPermit.ProjectTitle : string.Empty));

            CreateMap<Application, ApplicationBuildingPermitDetailDto>()
                .ForMember(dest => dest.BasicInformation, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.ProjectDetails, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.OwnerInformation, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.RequiredDocs, opt => opt.MapFrom(src => src.BuildingPermit != null ? src.BuildingPermit.AppInfo : null))
                .ForMember(dest => dest.BuildingPermitTechDocs, opt => opt.MapFrom(src => src.BuildingPermit != null ? src.BuildingPermit.TechDoc : null));

            // Mapping for BuildingPermitTechDoc
            CreateMap<BuildingPermitTechDoc, BuildingPermitTechDocDto>()
                .ForMember(dest => dest.TechDocIoCPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocIoCPlans)))
                .ForMember(dest => dest.TechDocSEPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocSEPlans)))
                .ForMember(dest => dest.TechDocEEPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocEEPlans)))
                .ForMember(dest => dest.TechDocSPPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocSPPlans)))
                .ForMember(dest => dest.TechDocBOMCost, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocBOMCost)))
                .ForMember(dest => dest.TechDocSoW, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocSoW)))
                .ForMember(dest => dest.TechDocMEPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocMEPlans)))
                .ForMember(dest => dest.TechDocECEPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocECEPlans)));

            // Mapping for RequiredDocs
            CreateMap<BuildingPermitAppInfo, RequiredDocs>()
                .ForMember(dest => dest.ReqDocProofOwnership, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.ReqDocProofOwnership)))
                .ForMember(dest => dest.ReqDocBarangayClearance, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.ReqDocBarangayClearance)))
                .ForMember(dest => dest.ReqDocTaxDeclaration, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.ReqDocTaxDeclaration)))
                .ForMember(dest => dest.ReqDocRealPropTaxReceipt, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.ReqDocRealPropTaxReceipt)))
                .ForMember(dest => dest.ReqDocECCorCNC, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.ReqDocECCorCNC)))
                .ForMember(dest => dest.ReqDocSpecialClearances, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.ReqDocSpecialClearances)));

            // Mapping for BasicInformationDto
            CreateMap<Application, BasicInformationDto>()
                .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProjectDescription, opt => opt.MapFrom(src => src.BuildingPermit != null ? src.BuildingPermit.ProjectTitle : string.Empty))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
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
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.BuildingPermit != null && src.BuildingPermit.AppInfo != null 
                    ? src.BuildingPermit.AppInfo.FullName : string.Empty))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.BuildingPermit != null && src.BuildingPermit.AppInfo != null 
                    ? src.BuildingPermit.AppInfo.MailAddress : string.Empty))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.BuildingPermit != null && src.BuildingPermit.AppInfo != null 
                    ? src.BuildingPermit.AppInfo.ContactNo : string.Empty))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.BuildingPermit != null && src.BuildingPermit.AppInfo != null 
                    ? src.BuildingPermit.AppInfo.Email : string.Empty));
        }
    }
}
