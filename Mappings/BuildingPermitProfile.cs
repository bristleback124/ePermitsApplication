using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities.BuildingPermit;

namespace ePermitsApp.Mappings
{
    public class BuildingPermitProfile : Profile
    {
        public BuildingPermitProfile()
        {
            CreateMap<BuildingPermit, BuildingPermitDto>().ReverseMap();
            CreateMap<BuildingPermitAppInfo, BuildingPermitAppInfoDto>().ReverseMap();
            CreateMap<BuildingPermitDesignProf, BuildingPermitDesignProfDto>().ReverseMap();
            CreateMap<BuildingPermitTechDoc, BuildingPermitTechDocDto>().ReverseMap();

            CreateMap<BuildingPermitCreateDto, BuildingPermit>()
                .ForMember(dest => dest.AppInfo, opt => opt.MapFrom(src => src.AppInfo))
                .ForMember(dest => dest.DesignProf, opt => opt.MapFrom(src => src.DesignProf))
                .ForMember(dest => dest.TechDoc, opt => opt.MapFrom(src => src.TechDoc));

            CreateMap<BuildingPermitAppInfoCreateDto, BuildingPermitAppInfo>()
                .ForMember(dest => dest.ReqDocProofOwnership, opt => opt.Ignore())
                .ForMember(dest => dest.ReqDocBarangayClearance, opt => opt.Ignore())
                .ForMember(dest => dest.ReqDocTaxDeclaration, opt => opt.Ignore())
                .ForMember(dest => dest.ReqDocRealPropTaxReceipt, opt => opt.Ignore())
                .ForMember(dest => dest.ReqDocECCorCNC, opt => opt.Ignore())
                .ForMember(dest => dest.ReqDocSpecialClearances, opt => opt.Ignore());

            CreateMap<BuildingPermitDesignProfCreateDto, BuildingPermitDesignProf>();

            CreateMap<BuildingPermitTechDocCreateDto, BuildingPermitTechDoc>()
                .ForMember(dest => dest.TechDocIoCPlans, opt => opt.Ignore())
                .ForMember(dest => dest.TechDocSEPlans, opt => opt.Ignore())
                .ForMember(dest => dest.TechDocEEPlans, opt => opt.Ignore())
                .ForMember(dest => dest.TechDocSPPlans, opt => opt.Ignore())
                .ForMember(dest => dest.TechDocBOMCost, opt => opt.Ignore())
                .ForMember(dest => dest.TechDocSoW, opt => opt.Ignore())
                .ForMember(dest => dest.TechDocMEPlans, opt => opt.Ignore())
                .ForMember(dest => dest.TechDocECEPlans, opt => opt.Ignore());
        }
    }
}
