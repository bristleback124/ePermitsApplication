using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Mappings
{
    public class PermitApplicationTypeProfile : Profile
    {
        public PermitApplicationTypeProfile()
        {
            CreateMap<PermitApplicationType, PermitApplicationTypeDto>().ReverseMap();
            CreateMap<CreatePermitApplicationTypeDto, PermitApplicationType>();
            CreateMap<UpdatePermitApplicationTypeDto, PermitApplicationType>();
        }
    }
}
