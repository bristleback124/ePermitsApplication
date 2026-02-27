using AutoMapper;
using ePermits.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IMapper _mapper;

        public ApplicationService(IApplicationRepository applicationRepository, IMapper mapper)
        {
            _applicationRepository = applicationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ApplicationDtoShort>> GetApplicationsByUserIdAsync(int userId)
        {
            var applications = await _applicationRepository.GetByUserIdDetailedAsync(userId);
            return _mapper.Map<IEnumerable<ApplicationDtoShort>>(applications);
        }

        public async Task<ApplicationBuildingPermitDetailDto?> GetApplicationBuildingPermitById(int id)
        {
            var application = await _applicationRepository.GetByIdBuildingPermitDetailedAsync(id);
            if (application == null) return null;

            return _mapper.Map<ApplicationBuildingPermitDetailDto>(application);
        }
    }
}
