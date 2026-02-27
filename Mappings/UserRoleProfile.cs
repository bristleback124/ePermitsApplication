using AutoMapper;
using ePermits.Models;
using ePermitsApp.DTOs;

namespace ePermitsApp.Mappings
{
    public class UserRoleProfile : Profile
    {
        public UserRoleProfile()
        {
            CreateMap<UserRole, UserRoleDto>().ReverseMap();
            CreateMap<CreateUserRoleDto, UserRole>();
            CreateMap<UpdateUserRoleDto, UserRole>();
        }
    }
}
