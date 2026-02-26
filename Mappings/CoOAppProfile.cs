using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities.CoOApp;

namespace ePermitsApp.Mappings
{
    public class CoOAppProfile : Profile
    {
        public CoOAppProfile()
        {
            CreateMap<CoOApp, CoOAppDto>().ReverseMap();
            CreateMap<CoOAppProf, CoOAppProfDto>().ReverseMap();
            CreateMap<CoOAppReqDoc, CoOAppReqDocDto>().ReverseMap();

            CreateMap<CoOAppCreateDto, CoOApp>()
                .ForMember(dest => dest.CoOAppProf, opt => opt.MapFrom(src => src.CoOAppProf))
                .ForMember(dest => dest.CoOAppReqDoc, opt => opt.MapFrom(src => src.CoOAppReqDoc));

            CreateMap<CoOAppProfCreateDto, CoOAppProf>();

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
