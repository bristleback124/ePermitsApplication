using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Mappings
{
    public class LGUProfile : Profile
    {
        public LGUProfile()
        {
            CreateMap<LGU, LGUDto>()
                .ForMember(dest => dest.ProvinceName,
                opt => opt.MapFrom(src => src.Province.ProvinceName));

            CreateMap<CreateLGUDto, LGU>();
            CreateMap<UpdateLGUDto, LGU>();
        }
    }
}
