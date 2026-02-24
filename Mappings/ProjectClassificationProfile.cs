using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Mappings
{
    public class ProjectClassificationProfile : Profile
    {
        public ProjectClassificationProfile()
        {
            CreateMap<ProjectClassification, ProjectClassificationDto>().ReverseMap();
            CreateMap<CreateProjectClassificationDto, ProjectClassification>();
            CreateMap<UpdateProjectClassificationDto, ProjectClassification>();
        }
    }
}
