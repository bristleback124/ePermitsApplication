using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Mappings
{
    public class ApplicantTypeProfile : Profile
    {
        public ApplicantTypeProfile() 
        {
            CreateMap<ApplicantType, ApplicantTypeDto>().ReverseMap();
            CreateMap<CreateApplicantTypeDto, ApplicantType>();
            CreateMap<UpdateApplicantTypeDto, ApplicantType>();
        }
    }
}
