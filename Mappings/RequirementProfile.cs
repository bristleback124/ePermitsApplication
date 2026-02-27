using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Mappings
{
    public class RequirementProfile : Profile
    {
        public RequirementProfile()
        {
            CreateMap<Requirement, RequirementDto>()
                .ForMember(dest => dest.ReqCatDesc,
                    opt => opt.MapFrom(src => src.RequirementCategory.ReqCatDesc))
                .ForMember(dest => dest.ReqClassDesc,
                    opt => opt.MapFrom(src => src.RequirementCategory.RequirementClassification.ReqClassDesc));

            CreateMap<CreateRequirementDto, Requirement>();
            CreateMap<UpdateRequirementDto, Requirement>();
        }
    }
}
