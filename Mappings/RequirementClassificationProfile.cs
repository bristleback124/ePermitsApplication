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
        }
    }
}
