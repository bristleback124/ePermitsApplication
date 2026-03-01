using AutoMapper;
using ePermits.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Services.Interfaces;
using ePermits.Models;

namespace ePermitsApp.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _applicationRepository = applicationRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ApplicationDtoShort>> GetApplicationsByUserIdAsync(int userId)
        {
            var applications = await _applicationRepository.GetByUserIdDetailedAsync(userId);
            return _mapper.Map<IEnumerable<ApplicationDtoShort>>(applications);
        }

        public async Task<IEnumerable<ReviewerDashboardItemDto>> GetReviewerDashboardAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            var applications = await _applicationRepository.GetDashboardDetailedAsync();

            if (IsDepartmentUser(currentUser) && currentUser!.DepartmentId.HasValue)
            {
                applications = applications
                    .Where(a => a.DepartmentReviews.Any(r => r.DepartmentId == currentUser.DepartmentId.Value))
                    .ToList();
            }

            var result = _mapper.Map<List<ReviewerDashboardItemDto>>(applications);
            TrimDepartmentReviewsForCurrentUser(currentUser, result);

            return result;
        }

        public async Task<ApplicationBuildingPermitDetailDto?> GetApplicationBuildingPermitById(int id)
        {
            var application = await _applicationRepository.GetByIdBuildingPermitDetailedAsync(id);
            if (application == null) return null;
            var currentUser = await GetCurrentUserAsync();

            if (!CanAccessApplication(currentUser, application))
            {
                return null;
            }

            var result = _mapper.Map<ApplicationBuildingPermitDetailDto>(application);
            TrimDepartmentReviewsForCurrentUser(currentUser, result.ReviewOffices);
            return result;
        }

        public async Task<ApplicationCoODetailDto?> GetApplicationCoOById(int id)
        {
            var application = await _applicationRepository.GetByIdCoODetailedAsync(id);
            if (application == null) return null;
            var currentUser = await GetCurrentUserAsync();

            if (!CanAccessApplication(currentUser, application))
            {
                return null;
            }

            var result = _mapper.Map<ApplicationCoODetailDto>(application);
            TrimDepartmentReviewsForCurrentUser(currentUser, result.ReviewOffices);
            return result;
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            if (!int.TryParse(_currentUserService.UserId, out var userId))
            {
                return null;
            }

            return await _userRepository.GetByIdAsync(userId);
        }

        private static bool IsDepartmentUser(User? user)
        {
            return string.Equals(user?.UserRole?.UserRoleDesc, "user", StringComparison.OrdinalIgnoreCase);
        }

        private static bool CanAccessApplication(User? currentUser, Application application)
        {
            if (!IsDepartmentUser(currentUser) || !currentUser!.DepartmentId.HasValue)
            {
                return true;
            }

            return application.DepartmentReviews.Any(r => r.DepartmentId == currentUser.DepartmentId.Value);
        }

        private static void TrimDepartmentReviewsForCurrentUser(User? currentUser, List<ApplicationDepartmentReviewDto> reviews)
        {
            if (!IsDepartmentUser(currentUser) || !currentUser!.DepartmentId.HasValue)
            {
                return;
            }

            reviews.RemoveAll(r => r.DepartmentId != currentUser.DepartmentId.Value);
        }

        private static void TrimDepartmentReviewsForCurrentUser(User? currentUser, List<ReviewerDashboardItemDto> items)
        {
            if (!IsDepartmentUser(currentUser) || !currentUser!.DepartmentId.HasValue)
            {
                return;
            }

            foreach (var item in items)
            {
                item.ReviewOffices = item.ReviewOffices
                    .Where(r => r.DepartmentId == currentUser.DepartmentId.Value)
                    .ToList();
            }
        }
    }
}
