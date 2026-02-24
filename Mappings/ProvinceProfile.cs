using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Mappings
{
    public class ProvinceProfile : Profile
    {
        public ProvinceProfile()
        {
            CreateMap<Province, ProvinceDto>().ReverseMap();
            CreateMap<CreateProvinceDto, Province>();
            CreateMap<UpdateProvinceDto, Province>();
        }
    }
}
