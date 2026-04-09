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
using ePermitsApp.Data;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Services
{
    public class CoOAppService : ICoOAppService
    {
        private static readonly DateTime DraftPlaceholderDate = new(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly ICoOAppRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepository _userRepository;
        private readonly FileStorageSettings _fileSettings;
        private readonly IEmailService _emailService;
        private readonly IAdminEmailNotificationConfigService _adminEmailNotificationConfigService;
        private readonly IApplicationFormattedIdService _applicationFormattedIdService;
        private readonly ILogger<CoOAppService> _logger;
        private readonly ApplicationDbContext _dbContext;

        public CoOAppService(
            ICoOAppRepository repository,
            IMapper mapper,
            ICurrentUserService currentUser,
            IUserRepository userRepository,
            IOptions<FileStorageSettings> fileSettings,
            IEmailService emailService,
            IAdminEmailNotificationConfigService adminEmailNotificationConfigService,
            IApplicationFormattedIdService applicationFormattedIdService,
            ApplicationDbContext dbContext,
            ILogger<CoOAppService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userRepository = userRepository;
            _fileSettings = fileSettings.Value;
            _emailService = emailService;
            _adminEmailNotificationConfigService = adminEmailNotificationConfigService;
            _dbContext = dbContext;
            _applicationFormattedIdService = applicationFormattedIdService;
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

        public async Task<CoOApp> CreateAsync(CoOAppCreateDto dto, bool saveAsDraft = false)
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

            if (saveAsDraft)
            {
                NormalizeDraftValues(coOApp);
                await NormalizeDraftForeignKeysAsync(coOApp);
            }

            coOApp.Application = new Application
            {
                UserId = currentUserId,
                Type = ApplicationWorkflowDefinitions.PermitTypes.CertificateOfOccupancy,
                Status = saveAsDraft
                    ? ApplicationWorkflowDefinitions.OverallStatuses.Draft
                    : ApplicationWorkflowDefinitions.OverallStatuses.Submitted,
                CreatedAt = now,
                CreatedBy = _currentUser.UserName ?? "System",
                DepartmentReviews = saveAsDraft
                    ? new List<ApplicationDepartmentReview>()
                    : ApplicationWorkflowDefinitions
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

            if (!saveAsDraft)
            {
                await _applicationFormattedIdService.AssignFormattedIdAsync(coOApp.Application);
            }

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

            if (!saveAsDraft)
            {
                ValidateRequiredSubmission(coOApp);
            }

            // Update again with file paths
            _repository.Update(coOApp);
            await _repository.SaveChangesAsync();

            if (!saveAsDraft)
            {
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
            }

            return coOApp;
        }

        public async Task<(bool Success, string Message, CoOApp? CoOApp)> UpdateByApplicationIdAsync(int applicationId, CoOAppUpdateDto dto, bool saveAsDraft = false)
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
            var wasDraft = string.Equals(
                coOApp.Application.Status,
                ApplicationWorkflowDefinitions.OverallStatuses.Draft,
                StringComparison.OrdinalIgnoreCase);

            _mapper.Map(dto, coOApp);
            _mapper.Map(dto.CoOAppProf, coOApp.CoOAppProf);
            coOApp.UpdatedAt = now;
            coOApp.UpdatedBy = currentUserId;
            coOApp.CoOAppProf.UpdatedAt = now;
            coOApp.CoOAppProf.UpdatedBy = currentUserId;
            coOApp.CoOAppReqDoc.UpdatedAt = now;
            coOApp.CoOAppReqDoc.UpdatedBy = currentUserId;

            if (saveAsDraft)
            {
                NormalizeDraftValues(coOApp);
                await NormalizeDraftForeignKeysAsync(coOApp);
            }

            await UpdateSingleReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocBldgPermitSPlans), dto.CoOAppReqDoc.ReqDocBldgPermitSPlans, dto.CoOAppReqDoc.KeepReqDocBldgPermitSPlans, coOApp.Id, !saveAsDraft);
            await UpdateSingleReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocAsBuiltPlans), dto.CoOAppReqDoc.ReqDocAsBuiltPlans, dto.CoOAppReqDoc.KeepReqDocAsBuiltPlans, coOApp.Id, !saveAsDraft);
            await UpdateSingleReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocConsLogbook), dto.CoOAppReqDoc.ReqDocConsLogbook, dto.CoOAppReqDoc.KeepReqDocConsLogbook, coOApp.Id, !saveAsDraft);
            await UpdateSingleReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocConsPhotos), dto.CoOAppReqDoc.ReqDocConsPhotos, dto.CoOAppReqDoc.KeepReqDocConsPhotos, coOApp.Id, !saveAsDraft);
            await UpdateSingleReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocBrgyClearance), dto.CoOAppReqDoc.ReqDocBrgyClearance, dto.CoOAppReqDoc.KeepReqDocBrgyClearance, coOApp.Id, !saveAsDraft);
            await UpdateSingleReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocFSIC), dto.CoOAppReqDoc.ReqDocFSIC, dto.CoOAppReqDoc.KeepReqDocFSIC, coOApp.Id, !saveAsDraft);
            await UpdateSingleReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocOthers), dto.CoOAppReqDoc.ReqDocOthers, dto.CoOAppReqDoc.KeepReqDocOthers, coOApp.Id, false);

            if (saveAsDraft)
            {
                coOApp.Application.Status = ApplicationWorkflowDefinitions.OverallStatuses.Draft;
                coOApp.Application.FormattedId = string.Empty;
                coOApp.Application.DepartmentReviews.Clear();
            }
            else
            {
                EnsureSubmittedState(coOApp.Application, now);
                ValidateRequiredSubmission(coOApp);

                if (wasDraft)
                {
                    coOApp.Application.CreatedAt = now;
                    await _applicationFormattedIdService.AssignFormattedIdAsync(coOApp.Application);
                }
            }

            coOApp.Application.UpdatedAt = now;
            coOApp.Application.UpdatedBy = actor;

            _repository.Update(coOApp);
            await _repository.SaveChangesAsync();

            if (!saveAsDraft && wasDraft)
            {
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
            }

            return (true, saveAsDraft ? "Draft saved successfully" : "Application updated successfully", coOApp);
        }

        private async Task<string> SaveFileAsync(IFormFile? file, int permitId, string subFolder)
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

        private static void ValidateRequiredSubmission(CoOApp coOApp)
        {
            if (string.IsNullOrWhiteSpace(coOApp.BldgPermitNo)) throw new InvalidOperationException("Building permit number is required.");
            if (string.IsNullOrWhiteSpace(coOApp.ProjectTitle)) throw new InvalidOperationException("Project title is required.");
            if (string.IsNullOrWhiteSpace(coOApp.ProjLocLot)) throw new InvalidOperationException("Lot is required.");
            if (string.IsNullOrWhiteSpace(coOApp.ProjLocStreet)) throw new InvalidOperationException("Street is required.");
            if (coOApp.ProvinceId <= 0) throw new InvalidOperationException("Province is required.");
            if (coOApp.LGUId <= 0) throw new InvalidOperationException("LGU is required.");
            if (coOApp.BarangayId <= 0) throw new InvalidOperationException("Barangay is required.");
            if (coOApp.OccupancyNatureId <= 0) throw new InvalidOperationException("Occupancy nature is required.");
            if (coOApp.FloorArea <= 0) throw new InvalidOperationException("Floor area is required.");
            if (coOApp.NoOfStoreys <= 0) throw new InvalidOperationException("Number of storeys is required.");
            if (IsDraftPlaceholderDate(coOApp.CompletionDate)) throw new InvalidOperationException("Completion date is required.");
            if (coOApp.ApplicantTypeId <= 0) throw new InvalidOperationException("Applicant type is required.");
            if (string.IsNullOrWhiteSpace(coOApp.FullName)) throw new InvalidOperationException("Applicant full name is required.");
            if (string.IsNullOrWhiteSpace(coOApp.ContactNo)) throw new InvalidOperationException("Contact number is required.");
            if (string.IsNullOrWhiteSpace(coOApp.Email)) throw new InvalidOperationException("Email is required.");
            if (string.IsNullOrWhiteSpace(coOApp.TIN)) throw new InvalidOperationException("TIN is required.");
            if (string.IsNullOrWhiteSpace(coOApp.MailAddress)) throw new InvalidOperationException("Mailing address is required.");
            if (string.IsNullOrWhiteSpace(coOApp.DigitalSignature)) throw new InvalidOperationException("Digital signature is required.");
            if (IsDraftPlaceholderDate(coOApp.DateOfSignature)) throw new InvalidOperationException("Signature date is required.");

            if (coOApp.CoOAppProf == null) throw new InvalidOperationException("Professional information is required.");
            if (string.IsNullOrWhiteSpace(coOApp.CoOAppProf.IoCFullName)) throw new InvalidOperationException("Architect / engineer is required.");
            if (string.IsNullOrWhiteSpace(coOApp.CoOAppProf.IoCPRCNo)) throw new InvalidOperationException("Architect / engineer PRC number is required.");
            if (string.IsNullOrWhiteSpace(coOApp.CoOAppProf.IoCPTRNo)) throw new InvalidOperationException("Architect / engineer PTR number is required.");
            if (IsDraftPlaceholderDate(coOApp.CoOAppProf.IOCValidity)) throw new InvalidOperationException("Architect / engineer validity is required.");
            if (string.IsNullOrWhiteSpace(coOApp.CoOAppProf.EoRFullName)) throw new InvalidOperationException("Engineer of record is required.");
            if (string.IsNullOrWhiteSpace(coOApp.CoOAppProf.EoRPRCorPTRNo)) throw new InvalidOperationException("Engineer of record PRC / PTR number is required.");
            if (IsDraftPlaceholderDate(coOApp.CoOAppProf.EoRValidity)) throw new InvalidOperationException("Engineer of record validity is required.");
            if (string.IsNullOrWhiteSpace(coOApp.CoOAppProf.EoRSpecialization)) throw new InvalidOperationException("Engineer of record specialization is required.");

            if (coOApp.CoOAppReqDoc == null) throw new InvalidOperationException("Required documents are required.");
            if (string.IsNullOrWhiteSpace(coOApp.CoOAppReqDoc.ReqDocBldgPermitSPlans)) throw new InvalidOperationException("Approved building permit set of plans is required.");
            if (string.IsNullOrWhiteSpace(coOApp.CoOAppReqDoc.ReqDocAsBuiltPlans)) throw new InvalidOperationException("As-built plans are required.");
            if (string.IsNullOrWhiteSpace(coOApp.CoOAppReqDoc.ReqDocConsLogbook)) throw new InvalidOperationException("Construction logbook is required.");
            if (string.IsNullOrWhiteSpace(coOApp.CoOAppReqDoc.ReqDocConsPhotos)) throw new InvalidOperationException("Construction photos are required.");
            if (string.IsNullOrWhiteSpace(coOApp.CoOAppReqDoc.ReqDocBrgyClearance)) throw new InvalidOperationException("Barangay clearance is required.");
            if (string.IsNullOrWhiteSpace(coOApp.CoOAppReqDoc.ReqDocFSIC)) throw new InvalidOperationException("Fire safety inspection certificate is required.");
        }

        private static void EnsureSubmittedState(Application application, DateTime now)
        {
            application.Status = ApplicationWorkflowDefinitions.OverallStatuses.Submitted;

            if (application.DepartmentReviews.Count > 0)
            {
                return;
            }

            application.DepartmentReviews = ApplicationWorkflowDefinitions
                .GetRequiredDepartmentIds(ApplicationWorkflowDefinitions.PermitTypes.CertificateOfOccupancy)
                .Select(departmentId => new ApplicationDepartmentReview
                {
                    DepartmentId = departmentId,
                    Status = ApplicationWorkflowDefinitions.DepartmentStatuses.InQueue,
                    CreatedAt = now
                })
                .ToList();
        }

        private static void NormalizeDraftValues(CoOApp coOApp)
        {
            coOApp.BldgPermitNo ??= string.Empty;
            coOApp.ProjectTitle ??= string.Empty;
            coOApp.ProjLocLot ??= string.Empty;
            coOApp.ProjLocStreet ??= string.Empty;
            coOApp.FullName ??= string.Empty;
            coOApp.ContactNo ??= string.Empty;
            coOApp.Email ??= string.Empty;
            coOApp.TIN ??= string.Empty;
            coOApp.MailAddress ??= string.Empty;
            coOApp.DigitalSignature ??= string.Empty;
            coOApp.CompletionDate = NormalizeDraftDate(coOApp.CompletionDate);
            coOApp.DateOfSignature = NormalizeDraftDate(coOApp.DateOfSignature);

            if (coOApp.CoOAppProf == null)
            {
                return;
            }

            coOApp.CoOAppProf.IoCFullName ??= string.Empty;
            coOApp.CoOAppProf.IoCPRCNo ??= string.Empty;
            coOApp.CoOAppProf.IoCPTRNo ??= string.Empty;
            coOApp.CoOAppProf.EoRFullName ??= string.Empty;
            coOApp.CoOAppProf.EoRPRCorPTRNo ??= string.Empty;
            coOApp.CoOAppProf.EoRSpecialization ??= string.Empty;
            coOApp.CoOAppProf.IOCValidity = NormalizeDraftDate(coOApp.CoOAppProf.IOCValidity);
            coOApp.CoOAppProf.EoRValidity = NormalizeDraftDate(coOApp.CoOAppProf.EoRValidity);
        }

        private static DateTime NormalizeDraftDate(DateTime date)
        {
            return date == default ? DraftPlaceholderDate : date;
        }

        private static bool IsDraftPlaceholderDate(DateTime date)
        {
            return date == default || date == DraftPlaceholderDate;
        }

        private async Task NormalizeDraftForeignKeysAsync(CoOApp coOApp)
        {
            if (coOApp.ProvinceId <= 0)
            {
                coOApp.ProvinceId = await _dbContext.Provinces
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => x.Id)
                    .FirstAsync();
            }

            if (coOApp.LGUId <= 0)
            {
                coOApp.LGUId = await _dbContext.LGUs
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => x.Id)
                    .FirstAsync();
            }

            if (coOApp.BarangayId <= 0)
            {
                coOApp.BarangayId = await _dbContext.Barangays
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => x.Id)
                    .FirstAsync();
            }

            if (coOApp.OccupancyNatureId <= 0)
            {
                coOApp.OccupancyNatureId = await _dbContext.OccupancyNatures
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => x.Id)
                    .FirstAsync();
            }

            if (coOApp.ApplicantTypeId <= 0)
            {
                coOApp.ApplicantTypeId = await _dbContext.ApplicantTypes
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => x.Id)
                    .FirstAsync();
            }
        }

        private async Task<bool> CanEditAsync(Application? application)
        {
            if (application == null)
            {
                return false;
            }

            var currentUser = await GetCurrentUserAsync();
            if (IsGovernmentUser(currentUser))
            {
                return true;
            }

            if (!string.Equals(application.Status, ApplicationWorkflowDefinitions.OverallStatuses.Submitted, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(application.Status, ApplicationWorkflowDefinitions.OverallStatuses.Draft, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var currentUserId = currentUser?.Id ?? 0;
            return currentUserId > 0 && application.UserId == currentUserId;
        }

        private async Task<bool> CanViewFormAsync(Application? application)
        {
            if (application == null)
            {
                return false;
            }

            var currentUser = await GetCurrentUserAsync();
            if (IsGovernmentUser(currentUser))
            {
                return true;
            }

            var currentUserId = currentUser?.Id ?? 0;
            return currentUserId > 0 && application.UserId == currentUserId;
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            var currentUserId = TryGetCurrentUserId();
            if (currentUserId <= 0)
            {
                return null;
            }

            return await _userRepository.GetByIdAsync(currentUserId);
        }

        private static bool IsGovernmentUser(User? user)
        {
            var role = user?.UserRole?.UserRoleDesc;
            return string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "user", StringComparison.OrdinalIgnoreCase);
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
