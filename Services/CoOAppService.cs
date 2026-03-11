using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Entities.CoOApp;
using ePermits.Models;
using ePermitsApp.Helpers;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using ePermitsApp.Models.EmailModels;
using Microsoft.Extensions.Options;
using ePermits.Data;

namespace ePermitsApp.Services
{
    public class CoOAppService : ICoOAppService
    {
        private readonly ICoOAppRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepository _userRepository;
        private readonly FileStorageSettings _fileSettings;
        private readonly IEmailService _emailService;
        private readonly IAdminEmailNotificationConfigService _adminEmailNotificationConfigService;
        private readonly ILogger<CoOAppService> _logger;

        public CoOAppService(
            ICoOAppRepository repository,
            IMapper mapper,
            ICurrentUserService currentUser,
            IUserRepository userRepository,
            IOptions<FileStorageSettings> fileSettings,
            IEmailService emailService,
            IAdminEmailNotificationConfigService adminEmailNotificationConfigService,
            ILogger<CoOAppService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userRepository = userRepository;
            _fileSettings = fileSettings.Value;
            _emailService = emailService;
            _adminEmailNotificationConfigService = adminEmailNotificationConfigService;
            _logger = logger;
        }

        public async Task<PagedResult<CoOApp>> GetAllAsync(PaginationParams pagination)
        {
            return await _repository.GetAllAsync(pagination);
        }

        public async Task<CoOApp?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<CoOAppEditDto?> GetEditByApplicationIdAsync(int applicationId)
        {
            var coOApp = await _repository.GetByApplicationIdAsync(applicationId);
            if (!await CanEditAsync(coOApp?.Application))
            {
                return null;
            }

            return coOApp == null ? null : _mapper.Map<CoOAppEditDto>(coOApp);
        }

        public async Task<CoOAppEditDto?> GetFormByApplicationIdAsync(int applicationId)
        {
            var coOApp = await _repository.GetByApplicationIdAsync(applicationId);
            if (!await CanViewFormAsync(coOApp?.Application))
            {
                return null;
            }

            return coOApp == null ? null : _mapper.Map<CoOAppEditDto>(coOApp);
        }

        public async Task<CoOApp> CreateAsync(CoOAppCreateDto dto)
        {
            var coOApp = _mapper.Map<CoOApp>(dto);

            var now = DateTime.UtcNow;
            int currentUserId = 15;
            if (int.TryParse(_currentUser.UserId, out int id))
            {
                currentUserId = id;
            }

            coOApp.CreatedAt = now;
            coOApp.CreatedBy = currentUserId;

            coOApp.Application = new Application
            {
                UserId = currentUserId,
                Type = ApplicationWorkflowDefinitions.PermitTypes.CertificateOfOccupancy,
                Status = ApplicationWorkflowDefinitions.OverallStatuses.Submitted,
                CreatedAt = now,
                CreatedBy = _currentUser.UserName ?? "System",
                DepartmentReviews = ApplicationWorkflowDefinitions
                    .GetRequiredDepartmentIds(ApplicationWorkflowDefinitions.PermitTypes.CertificateOfOccupancy)
                    .Select(departmentId => new ApplicationDepartmentReview
                    {
                        DepartmentId = departmentId,
                        Status = ApplicationWorkflowDefinitions.DepartmentStatuses.InQueue,
                        CreatedAt = now
                    })
                    .ToList()
            };

            if (coOApp.CoOAppProf != null)
            {
                coOApp.CoOAppProf.CreatedAt = now;
                coOApp.CoOAppProf.CreatedBy = currentUserId;
            }

            if (coOApp.CoOAppReqDoc != null)
            {
                coOApp.CoOAppReqDoc.CreatedAt = now;
                coOApp.CoOAppReqDoc.CreatedBy = currentUserId;
            }

            await _repository.AddAsync(coOApp);
            await _repository.SaveChangesAsync();

            // Set formatted ID now that we have the auto-generated Id
            coOApp.Application.FormattedId = $"CO-{coOApp.Application.CreatedAt.Year}-{coOApp.Application.Id:D3}";

            // Save files
            if (coOApp.CoOAppReqDoc != null)
            {
                coOApp.CoOAppReqDoc.ReqDocBldgPermitSPlans = await SaveFileAsync(dto.CoOAppReqDoc.ReqDocBldgPermitSPlans, coOApp.Id, "req-docs");
                coOApp.CoOAppReqDoc.ReqDocAsBuiltPlans = await SaveFileAsync(dto.CoOAppReqDoc.ReqDocAsBuiltPlans, coOApp.Id, "req-docs");
                coOApp.CoOAppReqDoc.ReqDocConsLogbook = await SaveFileAsync(dto.CoOAppReqDoc.ReqDocConsLogbook, coOApp.Id, "req-docs");
                coOApp.CoOAppReqDoc.ReqDocConsPhotos = await SaveFileAsync(dto.CoOAppReqDoc.ReqDocConsPhotos, coOApp.Id, "req-docs");
                coOApp.CoOAppReqDoc.ReqDocBrgyClearance = await SaveFileAsync(dto.CoOAppReqDoc.ReqDocBrgyClearance, coOApp.Id, "req-docs");
                coOApp.CoOAppReqDoc.ReqDocFSIC = await SaveFileAsync(dto.CoOAppReqDoc.ReqDocFSIC, coOApp.Id, "req-docs");

                if (dto.CoOAppReqDoc.ReqDocOthers != null)
                    coOApp.CoOAppReqDoc.ReqDocOthers = await SaveFileAsync(dto.CoOAppReqDoc.ReqDocOthers, coOApp.Id, "req-docs");
            }

            // Update again with file paths
            _repository.Update(coOApp);
            await _repository.SaveChangesAsync();

            // Send notification emails
            var applicantName = coOApp.FullName ?? "Unknown Applicant";
            await SendAdminNotificationEmailsAsync(
                coOApp.Application,
                applicantName,
                "Certificate of Occupancy");
            await SendApplicantSubmissionEmailAsync(
                coOApp.Email,
                applicantName,
                coOApp.Application,
                "Certificate of Occupancy");

            return coOApp;
        }

        public async Task<(bool Success, string Message, CoOApp? CoOApp)> UpdateByApplicationIdAsync(int applicationId, CoOAppUpdateDto dto)
        {
            var coOApp = await _repository.GetByApplicationIdAsync(applicationId);
            if (coOApp?.Application == null || coOApp.CoOAppProf == null || coOApp.CoOAppReqDoc == null)
            {
                return (false, "Application not found", null);
            }

            if (!await CanEditAsync(coOApp.Application))
            {
                return (false, "This application can no longer be edited", null);
            }

            var now = DateTime.UtcNow;
            var actor = _currentUser.UserName ?? "System";
            var currentUserId = TryGetCurrentUserId();

            _mapper.Map(dto, coOApp);
            _mapper.Map(dto.CoOAppProf, coOApp.CoOAppProf);
            coOApp.UpdatedAt = now;
            coOApp.UpdatedBy = currentUserId;
            coOApp.CoOAppProf.UpdatedAt = now;
            coOApp.CoOAppProf.UpdatedBy = currentUserId;
            coOApp.CoOAppReqDoc.UpdatedAt = now;
            coOApp.CoOAppReqDoc.UpdatedBy = currentUserId;

            await UpdateSingleReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocBldgPermitSPlans), dto.CoOAppReqDoc.ReqDocBldgPermitSPlans, dto.CoOAppReqDoc.KeepReqDocBldgPermitSPlans, coOApp.Id, true);
            await UpdateSingleReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocAsBuiltPlans), dto.CoOAppReqDoc.ReqDocAsBuiltPlans, dto.CoOAppReqDoc.KeepReqDocAsBuiltPlans, coOApp.Id, true);
            await UpdateSingleReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocConsLogbook), dto.CoOAppReqDoc.ReqDocConsLogbook, dto.CoOAppReqDoc.KeepReqDocConsLogbook, coOApp.Id, true);
            await UpdateSingleReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocConsPhotos), dto.CoOAppReqDoc.ReqDocConsPhotos, dto.CoOAppReqDoc.KeepReqDocConsPhotos, coOApp.Id, true);
            await UpdateSingleReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocBrgyClearance), dto.CoOAppReqDoc.ReqDocBrgyClearance, dto.CoOAppReqDoc.KeepReqDocBrgyClearance, coOApp.Id, true);
            await UpdateSingleReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocFSIC), dto.CoOAppReqDoc.ReqDocFSIC, dto.CoOAppReqDoc.KeepReqDocFSIC, coOApp.Id, true);
            await UpdateSingleReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocOthers), dto.CoOAppReqDoc.ReqDocOthers, dto.CoOAppReqDoc.KeepReqDocOthers, coOApp.Id, false);

            coOApp.Application.UpdatedAt = now;
            coOApp.Application.UpdatedBy = actor;

            _repository.Update(coOApp);
            await _repository.SaveChangesAsync();

            return (true, "Application updated successfully", coOApp);
        }

        private async Task<string> SaveFileAsync(IFormFile file, int permitId, string subFolder)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var folderPath = Path.Combine(_fileSettings.BasePath, "permits", permitId.ToString(), subFolder);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        private async Task UpdateSingleReqDocAsync(object target, string propertyName, IFormFile? newFile, bool keepExisting, int permitId, bool required)
        {
            var property = target.GetType().GetProperty(propertyName)!;
            var currentValue = property.GetValue(target) as string;

            if (newFile != null)
            {
                property.SetValue(target, await SaveFileAsync(newFile, permitId, "req-docs"));
                return;
            }

            if (keepExisting && !string.IsNullOrWhiteSpace(currentValue))
            {
                return;
            }

            if (required)
            {
                throw new InvalidOperationException($"{propertyName} is required.");
            }

            property.SetValue(target, string.Empty);
        }

        private async Task<bool> CanEditAsync(Application? application)
        {
            if (application == null)
            {
                return false;
            }

            if (await IsAdminAsync())
            {
                return true;
            }

            if (!string.Equals(application.Status, ApplicationWorkflowDefinitions.OverallStatuses.Submitted, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var currentUserId = TryGetCurrentUserId();
            return currentUserId > 0 && application.UserId == currentUserId;
        }

        private async Task<bool> CanViewFormAsync(Application? application)
        {
            if (application == null)
            {
                return false;
            }

            if (await IsAdminAsync())
            {
                return true;
            }

            var currentUserId = TryGetCurrentUserId();
            return currentUserId > 0 && application.UserId == currentUserId;
        }

        private async Task<bool> IsAdminAsync()
        {
            var currentUserId = TryGetCurrentUserId();
            if (currentUserId <= 0)
            {
                return false;
            }

            var user = await _userRepository.GetByIdAsync(currentUserId);
            return string.Equals(user?.UserRole?.UserRoleDesc, "admin", StringComparison.OrdinalIgnoreCase);
        }

        private int TryGetCurrentUserId()
        {
            return int.TryParse(_currentUser.UserId, out var id) ? id : 0;
        }

        private async Task SendAdminNotificationEmailsAsync(Application app, string applicantName, string appTypeLabel)
        {
            try
            {
                var emails = await _adminEmailNotificationConfigService.GetRecipientEmailsAsync(app.Type);
                if (emails.Count == 0) return;

                var to = emails.First();
                var cc = emails.Skip(1).ToList();

                await _emailService.SendTemplatedEmailAsync(
                    to,
                    cc,
                    $"New {appTypeLabel} Application: {app.FormattedId}",
                    "AdminApplicationSubmitted",
                    new AdminApplicationSubmittedModel
                    {
                        AdminName = "Admin",
                        ApplicantName = applicantName,
                        ApplicationType = appTypeLabel,
                        FormattedId = app.FormattedId,
                        SubmittedAt = app.CreatedAt
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send admin notification emails for application {FormattedId}", app.FormattedId);
            }
        }

        private async Task SendApplicantSubmissionEmailAsync(string? email, string applicantName, Application app, string appTypeLabel)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) return;

                await _emailService.SendTemplatedEmailAsync(
                    email,
                    $"Your {appTypeLabel} Application Has Been Submitted — {app.FormattedId}",
                    "ApplicationSubmitted",
                    new ApplicationSubmittedModel
                    {
                        ApplicantName = applicantName,
                        ApplicationType = appTypeLabel,
                        FormattedId = app.FormattedId,
                        SubmittedAt = app.CreatedAt
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send applicant submission email for application {FormattedId}", app.FormattedId);
            }
        }
    }
}
