using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Mappings
{
    public class RequirementClassificationProfile : Profile
    {
        public RequirementClassificationProfile()
        {
            CreateMap<RequirementClassification, RequirementClassificationDto>().ReverseMap();
            CreateMap<CreateRequirementClassificationDto, RequirementClassification>();
            CreateMap<UpdateRequirementClassificationDto, RequirementClassification>();

            // Hierarchy mappings
            CreateMap<RequirementClassification, RequirementClassificationHierarchyDto>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.RequirementCategorys));
            CreateMap<RequirementCategory, RequirementCategoryHierarchyDto>();
            CreateMap<Requirement, RequirementItemDto>();
        }
    }
}
