using AutoMapper;
using ePermits.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Helpers;
using ePermitsApp.Models.EmailModels;
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
        private readonly IEmailService _emailService;
        private readonly ILogger<ApplicationService> _logger;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            IEmailService emailService,
            ILogger<ApplicationService> logger)
        {
            _applicationRepository = applicationRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
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

        public Task<ApplicationStatusOptionsDto> GetStatusOptionsAsync()
        {
            return Task.FromResult(new ApplicationStatusOptionsDto
            {
                OverallStatuses = ApplicationWorkflowDefinitions.OverallStatusOptions.ToList(),
                DepartmentStatuses = ApplicationWorkflowDefinitions.DepartmentStatusOptions.ToList()
            });
        }

        public async Task<IEnumerable<ReviewAssignableUserDto>> GetAssignableReviewersAsync(int departmentId)
        {
            var users = await _userRepository.GetAssignableReviewersAsync(departmentId);
            return users.Select(u => new ReviewAssignableUserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.UserProfile != null
                    ? $"{u.UserProfile.FirstName} {u.UserProfile.LastName}".Trim()
                    : u.Username,
                Role = u.UserRole?.UserRoleDesc ?? string.Empty,
                DepartmentId = u.DepartmentId,
                DepartmentName = u.Department?.DepartmentName
            });
        }

        public async Task<(bool Success, string Message, ApplicationDepartmentReviewDto? Review)> AssignReviewerAsync(int applicationId, int departmentId, AssignApplicationReviewerDto dto)
        {
            var currentUser = await GetCurrentUserAsync();
            var review = await _applicationRepository.GetDepartmentReviewAsync(applicationId, departmentId);
            if (review == null)
            {
                return (false, "Department review not found", null);
            }

            var reviewer = await _userRepository.GetByIdAsync(dto.ReviewerId);
            if (reviewer == null || reviewer.UserRole == null)
            {
                return (false, "Reviewer not found", null);
            }

            var reviewerRole = reviewer.UserRole.UserRoleDesc;
            var isAdmin = string.Equals(reviewerRole, "admin", StringComparison.OrdinalIgnoreCase);
            var isDepartmentUser = string.Equals(reviewerRole, "user", StringComparison.OrdinalIgnoreCase);

            if (!isAdmin && !isDepartmentUser)
            {
                return (false, "Reviewer must have admin or user role", null);
            }

            if (isDepartmentUser && reviewer.DepartmentId != departmentId)
            {
                return (false, "Reviewer must belong to the same department", null);
            }

            review.AssignedReviewerId = reviewer.Id;
            review.AssignedAt = DateTime.UtcNow;
            review.UpdatedAt = DateTime.UtcNow;
            review.Application!.UpdatedAt = DateTime.UtcNow;
            review.Application.UpdatedBy = currentUser?.Username ?? "System";

            await _applicationRepository.UpdateAsync(review.Application!);

            await SendReviewerAssignedEmailAsync(reviewer, review);

            var updatedReview = await _applicationRepository.GetDepartmentReviewAsync(applicationId, departmentId);
            return updatedReview == null
                ? (true, "Reviewer assigned successfully", null)
                : (true, "Reviewer assigned successfully", _mapper.Map<ApplicationDepartmentReviewDto>(updatedReview));
        }

        public async Task<(bool Success, string Message, ApplicationDepartmentReviewDto? Review)> UpdateDepartmentReviewStatusAsync(int applicationId, int departmentId, UpdateApplicationDepartmentReviewStatusDto dto)
        {
            var review = await _applicationRepository.GetDepartmentReviewAsync(applicationId, departmentId);
            if (review == null || review.Application == null)
            {
                return (false, "Department review not found", null);
            }

            if (!ApplicationWorkflowDefinitions.IsValidDepartmentStatus(dto.Status))
            {
                return (false, "Invalid department status", null);
            }

            var currentUser = await GetCurrentUserAsync();
            if (!CanUpdateDepartmentReview(currentUser, review))
            {
                return (false, "You are not allowed to update this department review", null);
            }

            review.Status = dto.Status;
            review.UpdatedAt = DateTime.UtcNow;
            review.Application.UpdatedAt = DateTime.UtcNow;
            review.Application.UpdatedBy = currentUser?.Username ?? "System";

            await _applicationRepository.UpdateAsync(review.Application);

            await SendStatusUpdateEmailAsync(applicationId, dto.Status);

            var updatedReview = await _applicationRepository.GetDepartmentReviewAsync(applicationId, departmentId);
            return updatedReview == null
                ? (true, "Department status updated successfully", null)
                : (true, "Department status updated successfully", _mapper.Map<ApplicationDepartmentReviewDto>(updatedReview));
        }

        public async Task<(bool Success, string Message)> UpdateOverallStatusAsync(int applicationId, UpdateApplicationOverallStatusDto dto)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
            {
                return (false, "Application not found");
            }

            if (!ApplicationWorkflowDefinitions.IsValidOverallStatus(dto.Status))
            {
                return (false, "Invalid overall status");
            }

            var currentUser = await GetCurrentUserAsync();
            application.Status = dto.Status;
            application.UpdatedAt = DateTime.UtcNow;
            application.UpdatedBy = currentUser?.Username ?? "System";

            await _applicationRepository.UpdateAsync(application);

            await SendStatusUpdateEmailAsync(applicationId, dto.Status);

            return (true, "Overall status updated successfully");
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

        private static bool IsAdmin(User? user)
        {
            return string.Equals(user?.UserRole?.UserRoleDesc, "admin", StringComparison.OrdinalIgnoreCase);
        }

        private static bool CanAccessApplication(User? currentUser, Application application)
        {
            if (!IsDepartmentUser(currentUser) || !currentUser!.DepartmentId.HasValue)
            {
                return true;
            }

            return application.DepartmentReviews.Any(r => r.DepartmentId == currentUser.DepartmentId.Value);
        }

        private static bool CanUpdateDepartmentReview(User? currentUser, ApplicationDepartmentReview review)
        {
            if (currentUser == null || review.AssignedReviewerId != currentUser.Id)
            {
                return false;
            }

            if (IsAdmin(currentUser))
            {
                return true;
            }

            if (!IsDepartmentUser(currentUser) || !currentUser.DepartmentId.HasValue)
            {
                return false;
            }

            if (currentUser.DepartmentId.Value != review.DepartmentId)
            {
                return false;
            }

            return true;
        }

        private static void TrimDepartmentReviewsForCurrentUser(User? currentUser, List<ApplicationDepartmentReviewDto> reviews)
        {
            if (!IsDepartmentUser(currentUser) || !currentUser!.DepartmentId.HasValue)
            {
                return;
            }

            reviews.RemoveAll(r => r.DepartmentId != currentUser.DepartmentId.Value);
        }

        private async Task SendStatusUpdateEmailAsync(int applicationId, string newStatus)
        {
            try
            {
                var application = await _applicationRepository.GetByIdWithApplicantInfoAsync(applicationId);
                if (application == null) return;

                string? email = null;
                string? applicantName = null;
                string applicationType;

                if (application.Type == ApplicationWorkflowDefinitions.PermitTypes.BuildingPermit
                    && application.BuildingPermit?.AppInfo != null)
                {
                    email = application.BuildingPermit.AppInfo.Email;
                    applicantName = application.BuildingPermit.AppInfo.FullName;
                    applicationType = "Building Permit";
                }
                else if (application.Type == ApplicationWorkflowDefinitions.PermitTypes.CertificateOfOccupancy
                    && application.CoOApp != null)
                {
                    email = application.CoOApp.Email;
                    applicantName = application.CoOApp.FullName;
                    applicationType = "Certificate of Occupancy";
                }
                else
                {
                    return;
                }

                if (string.IsNullOrEmpty(email)) return;

                await _emailService.SendTemplatedEmailAsync(
                    email,
                    $"Your {applicationType} Application Status Has Been Updated",
                    "ApplicationStatusUpdated",
                    new ApplicationStatusUpdatedModel
                    {
                        ApplicantName = applicantName,
                        ApplicationType = applicationType,
                        FormattedId = application.FormattedId,
                        NewStatus = newStatus,
                        UpdatedAt = DateTime.UtcNow
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send status update email for Application {ApplicationId}", applicationId);
            }
        }

        private async Task SendReviewerAssignedEmailAsync(User reviewer, ApplicationDepartmentReview review)
        {
            try
            {
                var email = reviewer.UserProfile?.Email;
                if (string.IsNullOrEmpty(email)) return;

                var reviewerName = reviewer.UserProfile != null
                    ? $"{reviewer.UserProfile.FirstName} {reviewer.UserProfile.LastName}".Trim()
                    : reviewer.Username;

                var applicationType = review.Application?.Type == ApplicationWorkflowDefinitions.PermitTypes.BuildingPermit
                    ? "Building Permit"
                    : review.Application?.Type == ApplicationWorkflowDefinitions.PermitTypes.CertificateOfOccupancy
                        ? "Certificate of Occupancy"
                        : "Application";

                await _emailService.SendTemplatedEmailAsync(
                    email,
                    $"You Have Been Assigned to Review {applicationType} {review.Application?.FormattedId}",
                    "ReviewerAssigned",
                    new ReviewerAssignedModel
                    {
                        ReviewerName = reviewerName,
                        ApplicationType = applicationType,
                        FormattedId = review.Application?.FormattedId ?? string.Empty,
                        DepartmentName = review.Department?.DepartmentName ?? string.Empty,
                        AssignedAt = review.AssignedAt ?? DateTime.UtcNow
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reviewer assigned email for Application {ApplicationId}", review.ApplicationId);
            }
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
