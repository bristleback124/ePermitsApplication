using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Mappings
{
    public class OccupancyNatureProfile : Profile
    {
        public OccupancyNatureProfile()
        {
            CreateMap<OccupancyNature, OccupancyNatureDto>().ReverseMap();
            CreateMap<CreateOccupancyNatureDto, OccupancyNature>();
            CreateMap<UpdateOccupancyNatureDto, OccupancyNature>();
        }
    }
}
