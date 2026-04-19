using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities.CoOApp;
using ePermitsApp.Helpers;

namespace ePermitsApp.Mappings
{
    public class CoOAppProfile : Profile
    {
        public CoOAppProfile()
        {
            CreateMap<CoOApp, CoOAppDto>().ReverseMap();
            CreateMap<CoOAppProf, CoOAppProfDto>().ReverseMap();
            CreateMap<CoOAppProf, CoOAppProfEditDto>();
            CreateMap<CoOAppReqDoc, CoOAppReqDocDto>()
                .ForMember(dest => dest.ReqDocBldgPermitSPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocBldgPermitSPlans)))
                .ForMember(dest => dest.ReqDocAsBuiltPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocAsBuiltPlans)))
                .ForMember(dest => dest.ReqDocConsLogbook, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocConsLogbook)))
                .ForMember(dest => dest.ReqDocConsPhotos, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocConsPhotos)))
                .ForMember(dest => dest.ReqDocBrgyClearance, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocBrgyClearance)))
                .ForMember(dest => dest.ReqDocFSIC, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocFSIC)))
                .ForMember(dest => dest.ReqDocOthers, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocOthers)));
            CreateMap<CoOAppReqDocDto, CoOAppReqDoc>()
                .ForMember(dest => dest.ReqDocBldgPermitSPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.ReqDocBldgPermitSPlans)))
                .ForMember(dest => dest.ReqDocAsBuiltPlans, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.ReqDocAsBuiltPlans)))
                .ForMember(dest => dest.ReqDocConsLogbook, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.ReqDocConsLogbook)))
                .ForMember(dest => dest.ReqDocConsPhotos, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.ReqDocConsPhotos)))
                .ForMember(dest => dest.ReqDocBrgyClearance, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.ReqDocBrgyClearance)))
                .ForMember(dest => dest.ReqDocFSIC, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.ReqDocFSIC)))
                .ForMember(dest => dest.ReqDocOthers, opt => opt.MapFrom(src => FilePathHelper.Serialize(src.ReqDocOthers)));
            CreateMap<CoOAppReqDoc, CoOAppReqDocEditDto>()
                .ForMember(dest => dest.ReqDocBldgPermitSPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocBldgPermitSPlans)))
                .ForMember(dest => dest.ReqDocAsBuiltPlans, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocAsBuiltPlans)))
                .ForMember(dest => dest.ReqDocConsLogbook, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocConsLogbook)))
                .ForMember(dest => dest.ReqDocConsPhotos, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocConsPhotos)))
                .ForMember(dest => dest.ReqDocBrgyClearance, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocBrgyClearance)))
                .ForMember(dest => dest.ReqDocFSIC, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocFSIC)))
                .ForMember(dest => dest.ReqDocOthers, opt => opt.MapFrom(src => FilePathHelper.Deserialize(src.ReqDocOthers)));

            CreateMap<CoOApp, CoOAppEditDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Application != null ? src.Application.Status : string.Empty));

            CreateMap<CoOAppCreateDto, CoOApp>()
                .ForMember(dest => dest.CoOAppProf, opt => opt.MapFrom(src => src.CoOAppProf))
                .ForMember(dest => dest.CoOAppReqDoc, opt => opt.MapFrom(src => src.CoOAppReqDoc));
            CreateMap<CoOAppUpdateDto, CoOApp>()
                .ForMember(dest => dest.CoOAppProf, opt => opt.Ignore())
                .ForMember(dest => dest.CoOAppReqDoc, opt => opt.Ignore());

            CreateMap<CoOAppProfCreateDto, CoOAppProf>();
            CreateMap<CoOAppProfUpdateDto, CoOAppProf>();

            CreateMap<CoOAppReqDocCreateDto, CoOAppReqDoc>()
                .ForMember(dest => dest.ReqDocBldgPermitSPlans, opt => opt.Ignore())
                .ForMember(dest => dest.ReqDocAsBuiltPlans, opt => opt.Ignore())
                .ForMember(dest => dest.ReqDocConsLogbook, opt => opt.Ignore())
                .ForMember(dest => dest.ReqDocConsPhotos, opt => opt.Ignore())
                .ForMember(dest => dest.ReqDocBrgyClearance, opt => opt.Ignore())
                .ForMember(dest => dest.ReqDocFSIC, opt => opt.Ignore())
                .ForMember(dest => dest.ReqDocOthers, opt => opt.Ignore());
        }
    }
}
