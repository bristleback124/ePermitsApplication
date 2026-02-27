using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Mappings
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentDto>()
                .ForMember(dest => dest.LGUName,
                    opt => opt.MapFrom(src => src.LGU != null ? src.LGU.LGUName : string.Empty))
                .ForMember(dest => dest.ProvinceName,
                    opt => opt.MapFrom(src =>
                        src.LGU != null && src.LGU.Province != null
                            ? src.LGU.Province.ProvinceName
                            : string.Empty));

            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();
        }
    }
}
