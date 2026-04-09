using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities.BuildingPermit;
using ePermitsApp.Helpers;

namespace ePermitsApp.Mappings
{
    public class BuildingPermitProfile : Profile
    {
        private static List<string> ParseAccessories(string? accessories)
        {
            if (string.IsNullOrWhiteSpace(accessories))
            {
                return new List<string>();
            }

            return accessories
                .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }

        public BuildingPermitProfile()
        {
            CreateMap<BuildingPermit, BuildingPermitDto>()
                .ForMember(dest => dest.Accessories, opt => opt.MapFrom(src => ParseAccessories(src.Accessories)));
            CreateMap<BuildingPermitDto, BuildingPermit>()
                .ForMember(dest => dest.Accessories, opt => opt.MapFrom(src => string.Join("|", src.Accessories)));
            
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
                .ForMember(dest => dest.TechDocStructuralAnalysisDesign, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocStructuralAnalysisDesign)))
                .ForMember(dest => dest.TechDocFireSafetyPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocFireSafetyPlans)))
                .ForMember(dest => dest.TechDocEnvironmentalDocuments, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocEnvironmentalDocuments)))
                .ForMember(dest => dest.TechDocSoilTestFieldDensityTest, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocSoilTestFieldDensityTest)))
                .ForMember(dest => dest.TechDocBOMCost, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocBOMCost)))
                .ForMember(dest => dest.TechDocSoW, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocSoW)))
                .ForMember(dest => dest.TechDocMEPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocMEPlans)))
                .ForMember(dest => dest.TechDocECEPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.TechDocECEPlans)));
            
            CreateMap<BuildingPermitTechDocDto, BuildingPermitTechDoc>()
                .ForMember(dest => dest.TechDocIoCPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocIoCPlans)))
                .ForMember(dest => dest.TechDocSEPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocSEPlans)))
                .ForMember(dest => dest.TechDocEEPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocEEPlans)))
                .ForMember(dest => dest.TechDocSPPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocSPPlans)))
                .ForMember(dest => dest.TechDocStructuralAnalysisDesign, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocStructuralAnalysisDesign)))
                .ForMember(dest => dest.TechDocFireSafetyPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocFireSafetyPlans)))
                .ForMember(dest => dest.TechDocEnvironmentalDocuments, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocEnvironmentalDocuments)))
                .ForMember(dest => dest.TechDocSoilTestFieldDensityTest, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocSoilTestFieldDensityTest)))
                .ForMember(dest => dest.TechDocBOMCost, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocBOMCost)))
                .ForMember(dest => dest.TechDocSoW, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocSoW)))
                .ForMember(dest => dest.TechDocMEPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocMEPlans)))
                .ForMember(dest => dest.TechDocECEPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.TechDocECEPlans)));

            CreateMap<BuildingPermitSupportingDoc, BuildingPermitSupportingDocDto>()
                .ForMember(dest => dest.SupportDocZoningClearance, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.SupportDocZoningClearance)))
                .ForMember(dest => dest.SupportDocLocationalClearance, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.SupportDocLocationalClearance)))
                .ForMember(dest => dest.SupportDocFireSafetyClearance, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.SupportDocFireSafetyClearance)))
                .ForMember(dest => dest.SupportDocHighwayClearance, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.SupportDocHighwayClearance)))
                .ForMember(dest => dest.SupportDocHeightClearance, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.SupportDocHeightClearance)))
                .ForMember(dest => dest.SupportDocECCorCNC, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.SupportDocECCorCNC)))
                .ForMember(dest => dest.SupportDocDENRClearance, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.SupportDocDENRClearance)))
                .ForMember(dest => dest.SupportDocSECRegistration, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.SupportDocSECRegistration)))
                .ForMember(dest => dest.SupportDocBoardResolution, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.SupportDocBoardResolution)))
                .ForMember(dest => dest.SupportDocHOAClearance, opt => opt.MapFrom(src => FilePathHelper.DeserializeSingle(src.SupportDocHOAClearance)));

            CreateMap<BuildingPermitSupportingDocDto, BuildingPermitSupportingDoc>()
                .ForMember(dest => dest.SupportDocZoningClearance, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.SupportDocZoningClearance)))
                .ForMember(dest => dest.SupportDocLocationalClearance, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.SupportDocLocationalClearance)))
                .ForMember(dest => dest.SupportDocFireSafetyClearance, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.SupportDocFireSafetyClearance)))
                .ForMember(dest => dest.SupportDocHighwayClearance, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.SupportDocHighwayClearance)))
                .ForMember(dest => dest.SupportDocHeightClearance, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.SupportDocHeightClearance)))
                .ForMember(dest => dest.SupportDocECCorCNC, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.SupportDocECCorCNC)))
                .ForMember(dest => dest.SupportDocDENRClearance, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.SupportDocDENRClearance)))
                .ForMember(dest => dest.SupportDocSECRegistration, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.SupportDocSECRegistration)))
                .ForMember(dest => dest.SupportDocBoardResolution, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.SupportDocBoardResolution)))
                .ForMember(dest => dest.SupportDocHOAClearance, opt => opt.MapFrom(src => FilePathHelper.SerializeSingle(src.SupportDocHOAClearance)));

            CreateMap<BuildingPermitCreateDto, BuildingPermit>()
                .ForMember(dest => dest.AppInfo, opt => opt.MapFrom(src => src.AppInfo))
                .ForMember(dest => dest.DesignProf, opt => opt.MapFrom(src => src.DesignProf))
                .ForMember(dest => dest.TechDoc, opt => opt.MapFrom(src => src.TechDoc))
                .ForMember(dest => dest.SupportingDoc, opt => opt.MapFrom(src => src.SupportingDoc))
                .ForMember(dest => dest.Accessories, opt => opt.MapFrom(src => string.Join("|", src.Accessories)));

            CreateMap<BuildingPermit, BuildingPermitEditDto>()
                .ForMember(dest => dest.Accessories, opt => opt.MapFrom(src => ParseAccessories(src.Accessories)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Application != null ? src.Application.Status : string.Empty));

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
                .ForMember(dest => dest.TechDocStructuralAnalysisDesign, opt => opt.Ignore())
                .ForMember(dest => dest.TechDocFireSafetyPlans, opt => opt.Ignore())
                .ForMember(dest => dest.TechDocEnvironmentalDocuments, opt => opt.Ignore())
                .ForMember(dest => dest.TechDocSoilTestFieldDensityTest, opt => opt.Ignore())
                .ForMember(dest => dest.TechDocBOMCost, opt => opt.Ignore())
                .ForMember(dest => dest.TechDocSoW, opt => opt.Ignore())
                .ForMember(dest => dest.TechDocMEPlans, opt => opt.Ignore())
                .ForMember(dest => dest.TechDocECEPlans, opt => opt.Ignore());

            CreateMap<BuildingPermitSupportingDocCreateDto, BuildingPermitSupportingDoc>()
                .ForMember(dest => dest.SupportDocZoningClearance, opt => opt.Ignore())
                .ForMember(dest => dest.SupportDocLocationalClearance, opt => opt.Ignore())
                .ForMember(dest => dest.SupportDocFireSafetyClearance, opt => opt.Ignore())
                .ForMember(dest => dest.SupportDocHighwayClearance, opt => opt.Ignore())
                .ForMember(dest => dest.SupportDocHeightClearance, opt => opt.Ignore())
                .ForMember(dest => dest.SupportDocECCorCNC, opt => opt.Ignore())
                .ForMember(dest => dest.SupportDocDENRClearance, opt => opt.Ignore())
                .ForMember(dest => dest.SupportDocSECRegistration, opt => opt.Ignore())
                .ForMember(dest => dest.SupportDocBoardResolution, opt => opt.Ignore())
                .ForMember(dest => dest.SupportDocHOAClearance, opt => opt.Ignore());
        }
    }
}
