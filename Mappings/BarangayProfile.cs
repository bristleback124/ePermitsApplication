using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Mappings
{
    public class BarangayProfile : Profile
    {
        public BarangayProfile()
        {
            CreateMap<Barangay, BarangayDto>()
                .ForMember(dest => dest.LGUName,
                    opt => opt.MapFrom(src => src.LGU.LGUName))
                .ForMember(dest => dest.ProvinceName,
                    opt => opt.MapFrom(src => src.LGU.Province.ProvinceName));

            CreateMap<CreateBarangayDto, Barangay>();
            CreateMap<UpdateBarangayDto, Barangay>();
        }
    }
}
