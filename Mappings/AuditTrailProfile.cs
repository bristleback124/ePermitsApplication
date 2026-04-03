using AutoMapper;
using ePermits.Models;
using ePermitsApp.DTOs;

namespace ePermitsApp.Mappings
{
    public class AuditTrailProfile : Profile
    {
        public AuditTrailProfile()
        {
            CreateMap<AuditTrail, AuditTrailDto>();
        }
    }
}
