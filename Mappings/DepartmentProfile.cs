using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Mappings
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentDto>();

            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();
        }
    }
}
