using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Mappings
{
    public class RequirementCategoryProfile : Profile
    {
        public RequirementCategoryProfile()
        {
            CreateMap<RequirementCategory, RequirementCategoryDto>()
                .ForMember(dest => dest.ReqClassDesc,
                opt => opt.MapFrom(src => src.RequirementClassification.ReqClassDesc));

            CreateMap<CreateRequirementCategoryDto, RequirementCategory>();
            CreateMap<UpdateRequirementCategoryDto, RequirementCategory>();
        }
    }
}
