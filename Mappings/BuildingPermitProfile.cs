using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities.BuildingPermit;
using ePermitsApp.Helpers;

namespace ePermitsApp.Mappings
{
    public class BuildingPermitProfile : Profile
    {
        public BuildingPermitProfile()
        {
            CreateMap<BuildingPermit, BuildingPermitDto>().ReverseMap();
            
            CreateMap<BuildingPermitAppInfo, BuildingPermitAppInfoDto>()
                .ForMember(dest => dest.ReqDocProofOwnership, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.ReqDocProofOwnership)))
                .ForMember(dest => dest.ReqDocBarangayClearance, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.ReqDocBarangayClearance)))
                .ForMember(dest => dest.ReqDocTaxDeclaration, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.ReqDocTaxDeclaration)))
                .ForMember(dest => dest.ReqDocRealPropTaxReceipt, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.ReqDocRealPropTaxReceipt)))
                .ForMember(dest => dest.ReqDocECCorCNC, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.ReqDocECCorCNC)))
                .ForMember(dest => dest.ReqDocSpecialClearances, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.ReqDocSpecialClearances)));

            CreateMap<BuildingPermitAppInfoDto, BuildingPermitAppInfo>()
                .ForMember(dest => dest.ReqDocProofOwnership, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.ReqDocProofOwnership)))
                .ForMember(dest => dest.ReqDocBarangayClearance, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.ReqDocBarangayClearance)))
                .ForMember(dest => dest.ReqDocTaxDeclaration, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.ReqDocTaxDeclaration)))
                .ForMember(dest => dest.ReqDocRealPropTaxReceipt, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.ReqDocRealPropTaxReceipt)))
                .ForMember(dest => dest.ReqDocECCorCNC, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.ReqDocECCorCNC)))
                .ForMember(dest => dest.ReqDocSpecialClearances, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.ReqDocSpecialClearances)));

            CreateMap<BuildingPermitDesignProf, BuildingPermitDesignProfDto>().ReverseMap();
            
            CreateMap<BuildingPermitTechDoc, BuildingPermitTechDocDto>()
                .ForMember(dest => dest.TechDocIoCPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocIoCPlans)))
                .ForMember(dest => dest.TechDocSEPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocSEPlans)))
                .ForMember(dest => dest.TechDocEEPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocEEPlans)))
                .ForMember(dest => dest.TechDocSPPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocSPPlans)))
                .ForMember(dest => dest.TechDocBOMCost, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocBOMCost)))
                .ForMember(dest => dest.TechDocSoW, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocSoW)))
                .ForMember(dest => dest.TechDocMEPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocMEPlans)))
                .ForMember(dest => dest.TechDocECEPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocECEPlans)));
            
            CreateMap<BuildingPermitTechDocDto, BuildingPermitTechDoc>()
                .ForMember(dest => dest.TechDocIoCPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocIoCPlans)))
                .ForMember(dest => dest.TechDocSEPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocSEPlans)))
                .ForMember(dest => dest.TechDocEEPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocEEPlans)))
                .ForMember(dest => dest.TechDocSPPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocSPPlans)))
                .ForMember(dest => dest.TechDocBOMCost, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocBOMCost)))
                .ForMember(dest => dest.TechDocSoW, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocSoW)))
                .ForMember(dest => dest.TechDocMEPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocMEPlans)))
                .ForMember(dest => dest.TechDocECEPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocECEPlans)));

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
