using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Mappings
{
    public class OwnershipTypeProfile : Profile
    {
        public OwnershipTypeProfile()
        {
            CreateMap<OwnershipType, OwnershipTypeDto>().ReverseMap();
            CreateMap<CreateOwnershipTypeDto, OwnershipType>();
            CreateMap<UpdateOwnershipTypeDto, OwnershipType>();
        }
    }
}
