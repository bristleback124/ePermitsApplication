using AutoMapper;
using ePermits.Data;
using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Helpers;
using ePermitsApp.Constants;
using ePermitsApp.Models.EmailModels;
using ePermitsApp.Services.Interfaces;
using ePermits.Models;
using System.Security.Cryptography;
using System.Text;

namespace ePermitsApp.Services
{
    public class ApplicationService : IApplicationService
    {
        private const int DepartmentUserRoleId = 2;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<ApplicationService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuditTrailService _auditTrailService;
        private readonly IWorkflowTransitionService _workflowTransitionService;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            IUserRepository userRepository,
            ApplicationDbContext dbContext,
            ICurrentUserService currentUserService,
            IMapper mapper,
            IEmailService emailService,
            ILogger<ApplicationService> logger,
            IAuditTrailService auditTrailService,
            IWorkflowTransitionService workflowTransitionService)
        {
            _applicationRepository = applicationRepository;
            _userRepository = userRepository;
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
            _auditTrailService = auditTrailService;
            _workflowTransitionService = workflowTransitionService;
        }

        public async Task<IEnumerable<ApplicationDtoShort>> GetApplicationsByUserIdAsync(int userId)
        {
            var currentUser = await GetCurrentUserAsync();
            if (!IsAdmin(currentUser) && currentUser?.Id != userId)
            {
                return Enumerable.Empty<ApplicationDtoShort>();
            }

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

            applications = applications
                .Where(a => !string.Equals(a.Status, ApplicationWorkflowDefinitions.OverallStatuses.Draft, StringComparison.OrdinalIgnoreCase))
                .ToList();

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
                OverallStatuses = ApplicationWorkflowDefinitions.AllStatuses.ToList(),
                DepartmentStatuses = ApplicationWorkflowDefinitions.DepartmentStatusOptions.ToList(),
                ReviewSubstatuses = ApplicationWorkflowDefinitions.ReviewSubstatusOptions.ToList()
            });
        }

        public async Task<IEnumerable<ReviewAssignableUserDto>> GetAssignableReviewersAsync(int departmentId)
        {
            await EnsureDummyReviewersAsync(departmentId);

            var users = await _userRepository.GetAssignableReviewersAsync(departmentId);
            var reviewers = users.Select(u => new ReviewAssignableUserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.UserProfile != null
                    ? $"{u.UserProfile.FirstName} {u.UserProfile.LastName}".Trim()
                    : u.Username,
                Role = u.UserRole?.UserRoleDesc ?? string.Empty,
                DepartmentId = u.DepartmentId,
                DepartmentName = u.Department?.DepartmentName,
                IsPlaceholder = false
            }).ToList();

            return reviewers;
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
            var isAdminRole = IsAdmin(reviewer);
            var isDepartmentUser = string.Equals(reviewerRole, ApplicationWorkflowDefinitions.Roles.User, StringComparison.OrdinalIgnoreCase);
            var isWorkflowRole = ApplicationWorkflowDefinitions.IsValidRole(reviewerRole);

            if (!isAdminRole && !isDepartmentUser && !isWorkflowRole)
            {
                return (false, "Reviewer must have a valid staff role", null);
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

            var previousStatus = review.Status;
            review.Status = dto.Status;
            review.UpdatedAt = DateTime.UtcNow;
            review.Application.UpdatedAt = DateTime.UtcNow;
            review.Application.UpdatedBy = currentUser?.Username ?? "System";

            await _applicationRepository.UpdateAsync(review.Application);

            await SendStatusUpdateEmailAsync(applicationId, dto.Status, departmentId: departmentId, previousStatus: previousStatus);

            var userId = int.TryParse(_currentUserService.UserId, out var uid) ? uid : 0;
            var userName = currentUser?.Username ?? "System";
            await _auditTrailService.LogAsync(
                applicationId,
                AuditActionTypes.StatusChange,
                $"Department status changed to {dto.Status}",
                $"Previous status: {previousStatus}",
                userId,
                userName);

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

            var currentUser = await GetCurrentUserAsync();
            var userRole = currentUser?.UserRole?.UserRoleDesc ?? string.Empty;

            // Validate transition through workflow service
            var (isValid, validationMessage) = _workflowTransitionService.ValidateTransition(
                application.Status, userRole, dto.Status);

            if (!isValid)
            {
                return (false, validationMessage);
            }

            if (string.Equals(dto.Status, ApplicationWorkflowDefinitions.OverallStatuses.ForFeeComputation, StringComparison.OrdinalIgnoreCase)
                && string.Equals(userRole, ApplicationWorkflowDefinitions.Roles.InitialReviewer, StringComparison.OrdinalIgnoreCase)
                && (!string.Equals(application.RequirementsReviewStatus, ApplicationWorkflowDefinitions.ReviewSubstatuses.Complete, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(application.TechnicalPlansReviewStatus, ApplicationWorkflowDefinitions.ReviewSubstatuses.Complete, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "Requirements and Technical Plans reviews must both be Complete before proceeding to fee computation");
            }

            var previousStatus = application.Status;
            application.Status = dto.Status;

            if (string.Equals(previousStatus, ApplicationWorkflowDefinitions.OverallStatuses.DeficiencyIssued, StringComparison.OrdinalIgnoreCase)
                && string.Equals(dto.Status, ApplicationWorkflowDefinitions.OverallStatuses.Submitted, StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(application.RequirementsReviewStatus, ApplicationWorkflowDefinitions.ReviewSubstatuses.DeficiencyIssued, StringComparison.OrdinalIgnoreCase))
                {
                    application.RequirementsReviewStatus = ApplicationWorkflowDefinitions.ReviewSubstatuses.ReviewNotStarted;
                }

                if (string.Equals(application.TechnicalPlansReviewStatus, ApplicationWorkflowDefinitions.ReviewSubstatuses.DeficiencyIssued, StringComparison.OrdinalIgnoreCase))
                {
                    application.TechnicalPlansReviewStatus = ApplicationWorkflowDefinitions.ReviewSubstatuses.ReviewNotStarted;
                }
            }

            if (dto.Reason != null)
                application.StatusReason = dto.Reason;
            application.UpdatedAt = DateTime.UtcNow;
            application.UpdatedBy = currentUser?.Username ?? "System";

            await _applicationRepository.UpdateAsync(application);

            await SendStatusUpdateEmailAsync(applicationId, dto.Status, previousStatus: previousStatus);

            var overallUserId = int.TryParse(_currentUserService.UserId, out var ouid) ? ouid : 0;
            var overallUserName = currentUser?.Username ?? "System";
            var details = string.IsNullOrWhiteSpace(dto.Reason)
                ? $"Previous status: {previousStatus}"
                : $"Previous status: {previousStatus}. Reason: {dto.Reason}";
            await _auditTrailService.LogAsync(
                applicationId,
                AuditActionTypes.StatusChange,
                $"Status changed to {dto.Status}",
                details,
                overallUserId,
                overallUserName);

            return (true, "Status updated successfully");
        }

        public async Task<(bool Success, string Message)> UpdateReviewSubstatusAsync(int applicationId, UpdateApplicationReviewSubstatusDto dto)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
            {
                return (false, "Application not found");
            }

            if (!ApplicationWorkflowDefinitions.IsValidReviewSubstatus(dto.Status))
            {
                return (false, "Invalid review substatus");
            }

            var currentUser = await GetCurrentUserAsync();
            var userRole = currentUser?.UserRole?.UserRoleDesc ?? string.Empty;
            var reviewType = dto.ReviewType.Trim().ToLowerInvariant();
            var canEditRequirements = reviewType == "requirements"
                && string.Equals(userRole, ApplicationWorkflowDefinitions.Roles.InitialReviewer, StringComparison.OrdinalIgnoreCase);
            var canEditTechnicalPlans = reviewType == "technical-plans"
                && string.Equals(userRole, ApplicationWorkflowDefinitions.Roles.TechnicalReviewer, StringComparison.OrdinalIgnoreCase);

            if (!canEditRequirements && !canEditTechnicalPlans)
            {
                return (false, "You are not allowed to update this review substatus");
            }

            if (string.Equals(dto.Status, ApplicationWorkflowDefinitions.ReviewSubstatuses.DeficiencyIssued, StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(dto.Reason))
            {
                return (false, "A deficiency reason is required");
            }

            var previousStatus = canEditRequirements
                ? application.RequirementsReviewStatus
                : application.TechnicalPlansReviewStatus;
            var previousOverallStatus = application.Status;

            if (canEditRequirements)
            {
                application.RequirementsReviewStatus = dto.Status;
            }
            else
            {
                application.TechnicalPlansReviewStatus = dto.Status;
            }

            if (string.Equals(dto.Status, ApplicationWorkflowDefinitions.ReviewSubstatuses.DeficiencyIssued, StringComparison.OrdinalIgnoreCase))
            {
                application.Status = ApplicationWorkflowDefinitions.OverallStatuses.DeficiencyIssued;
                application.StatusReason = dto.Reason;
            }

            application.UpdatedAt = DateTime.UtcNow;
            application.UpdatedBy = currentUser?.Username ?? "System";

            await _applicationRepository.UpdateAsync(application);

            if (string.Equals(dto.Status, ApplicationWorkflowDefinitions.ReviewSubstatuses.DeficiencyIssued, StringComparison.OrdinalIgnoreCase))
            {
                await SendStatusUpdateEmailAsync(applicationId, application.Status, previousStatus: previousOverallStatus);
            }

            var userId = int.TryParse(_currentUserService.UserId, out var uid) ? uid : 0;
            var reviewLabel = canEditRequirements ? "Requirements" : "Technical Plans";
            var details = string.IsNullOrWhiteSpace(dto.Reason)
                ? $"Previous {reviewLabel} review substatus: {previousStatus}"
                : $"Previous {reviewLabel} review substatus: {previousStatus}. Reason: {dto.Reason}";
            await _auditTrailService.LogAsync(
                applicationId,
                AuditActionTypes.StatusChange,
                $"{reviewLabel} review substatus changed to {dto.Status}",
                details,
                userId,
                currentUser?.Username ?? "System");

            return (true, "Review substatus updated successfully");
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            if (!int.TryParse(_currentUserService.UserId, out var userId))
            {
                return null;
            }

            return await _userRepository.GetByIdAsync(userId);
        }

        private async Task EnsureDummyReviewersAsync(int departmentId)
        {
            var reviewers = departmentId switch
            {
                ApplicationWorkflowDefinitions.DepartmentIds.OBO => new[]
                {
                    new DummyReviewerSeed("obo.rosa", "Rosa", "Martinez (OBO)", "obo.rosa@epermits.local", "09170001001"),
                    new DummyReviewerSeed("obo.miguel", "Miguel", "Torres (OBO)", "obo.miguel@epermits.local", "09170001002"),
                    new DummyReviewerSeed("obo.elena", "Elena", "Rodriguez (OBO)", "obo.elena@epermits.local", "09170001003"),
                    new DummyReviewerSeed("obo.andres", "Andres", "Villanueva (OBO)", "obo.andres@epermits.local", "09170001004"),
                    new DummyReviewerSeed("obo.carla", "Carla", "Domingo (OBO)", "obo.carla@epermits.local", "09170001005"),
                },
                ApplicationWorkflowDefinitions.DepartmentIds.CPDO => new[]
                {
                    new DummyReviewerSeed("cpdo.pedro", "Pedro", "Cruz (CPDO)", "cpdo.pedro@epermits.local", "09170002001"),
                    new DummyReviewerSeed("cpdo.ana", "Ana", "Lopez (CPDO)", "cpdo.ana@epermits.local", "09170002002"),
                    new DummyReviewerSeed("cpdo.carlos", "Carlos", "Wang (CPDO)", "cpdo.carlos@epermits.local", "09170002003"),
                    new DummyReviewerSeed("cpdo.lianne", "Lianne", "Fernandez (CPDO)", "cpdo.lianne@epermits.local", "09170002004"),
                    new DummyReviewerSeed("cpdo.marco", "Marco", "Reyes (CPDO)", "cpdo.marco@epermits.local", "09170002005"),
                },
                ApplicationWorkflowDefinitions.DepartmentIds.BFP => new[]
                {
                    new DummyReviewerSeed("bfp.maria", "Maria", "Santos (BFP)", "bfp.maria@epermits.local", "09170003001"),
                    new DummyReviewerSeed("bfp.john", "John", "Doe (BFP)", "bfp.john@epermits.local", "09170003002"),
                    new DummyReviewerSeed("bfp.lisa", "Lisa", "Garcia (BFP)", "bfp.lisa@epermits.local", "09170003003"),
                    new DummyReviewerSeed("bfp.kevin", "Kevin", "Ramos (BFP)", "bfp.kevin@epermits.local", "09170003004"),
                    new DummyReviewerSeed("bfp.nina", "Nina", "Aquino (BFP)", "bfp.nina@epermits.local", "09170003005"),
                },
                _ => Array.Empty<DummyReviewerSeed>()
            };

            if (reviewers.Length == 0)
            {
                return;
            }

            foreach (var reviewer in reviewers)
            {
                var existingUser = await _userRepository.GetByUsernameAsync(reviewer.Username);
                if (existingUser != null)
                {
                    continue;
                }

                var user = new User
                {
                    Username = reviewer.Username,
                    Password = HashPassword("password123"),
                    UserRoleId = DepartmentUserRoleId,
                    DepartmentId = departmentId,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow,
                    UserProfile = new UserProfile
                    {
                        FirstName = reviewer.FirstName,
                        MiddleName = string.Empty,
                        LastName = reviewer.LastName,
                        Email = reviewer.Email,
                        MobileNo = reviewer.MobileNo,
                        CreatedBy = "System",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                _dbContext.Users.Add(user);
            }

            await _dbContext.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool IsDepartmentUser(User? user)
        {
            return string.Equals(user?.UserRole?.UserRoleDesc, ApplicationWorkflowDefinitions.Roles.User, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsAdmin(User? user)
        {
            var role = user?.UserRole?.UserRoleDesc;
            return string.Equals(role, ApplicationWorkflowDefinitions.Roles.SuperAdmin, StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase) // backwards compat
                || string.Equals(role, ApplicationWorkflowDefinitions.Roles.SysAdmin, StringComparison.OrdinalIgnoreCase);
        }

        private sealed record DummyReviewerSeed(
            string Username,
            string FirstName,
            string LastName,
            string Email,
            string MobileNo);

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
            if (currentUser == null)
            {
                return false;
            }

            // Admin roles can update any department review
            if (IsAdmin(currentUser))
            {
                return true;
            }

            // All internal/staff roles can update department review status
            var role = currentUser.UserRole?.UserRoleDesc;
            if (ApplicationWorkflowDefinitions.IsValidRole(role ?? "")
                && !string.Equals(role, ApplicationWorkflowDefinitions.Roles.Applicant, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(role, ApplicationWorkflowDefinitions.Roles.Executive, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Legacy: department users assigned to this review
            if (review.AssignedReviewerId == currentUser.Id
                && IsDepartmentUser(currentUser)
                && currentUser.DepartmentId.HasValue
                && currentUser.DepartmentId.Value == review.DepartmentId)
            {
                return true;
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

        private async Task SendStatusUpdateEmailAsync(int applicationId, string newStatus, int? departmentId = null, string? previousStatus = null)
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

                // Resolve department info if a department-level update
                string? departmentName = null;
                string? departmentCode = null;
                if (departmentId.HasValue)
                {
                    var review = await _applicationRepository.GetDepartmentReviewAsync(applicationId, departmentId.Value);
                    departmentName = review?.Department?.DepartmentName;
                    departmentCode = review?.Department?.DepartmentCode;
                }

                // Resolve current user's name
                string? updatedByName = null;
                var currentUser = await GetCurrentUserAsync();
                if (currentUser?.UserProfile != null)
                {
                    updatedByName = $"{currentUser.UserProfile.FirstName} {currentUser.UserProfile.LastName}".Trim();
                }

                var isApproved = string.Equals(newStatus, ApplicationWorkflowDefinitions.OverallStatuses.ApprovedForIssuance, StringComparison.OrdinalIgnoreCase);

                string subject;
                string templateName;

                if (isApproved && !departmentId.HasValue)
                {
                    subject = $"Congratulations! Your {applicationType} Application Has Been Approved — {application.FormattedId}";
                    templateName = "ApplicationApproved";
                }
                else if (!string.IsNullOrEmpty(departmentName))
                {
                    subject = $"Your {applicationType} Application Status Updated by {departmentName}";
                    templateName = "ApplicationStatusUpdated";
                }
                else
                {
                    subject = $"Your {applicationType} Application Status Has Been Updated";
                    templateName = "ApplicationStatusUpdated";
                }

                await _emailService.SendTemplatedEmailAsync(
                    email,
                    subject,
                    templateName,
                    new ApplicationStatusUpdatedModel
                    {
                        ApplicantName = applicantName,
                        ApplicationType = applicationType,
                        FormattedId = application.FormattedId,
                        PreviousStatus = previousStatus,
                        NewStatus = newStatus,
                        UpdatedAt = DateTime.UtcNow,
                        DepartmentName = departmentName,
                        DepartmentCode = departmentCode,
                        UpdatedBy = updatedByName
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
