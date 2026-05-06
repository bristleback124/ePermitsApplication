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
        private readonly IFileStorageService _fileStorageService;
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
            IFileStorageService fileStorageService,
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
            _fileStorageService = fileStorageService;
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

        public async Task<CoOApp> CreateAsync(CoOAppCreateDto dto, bool saveAsDraft = false, int? applicantId = null)
        {
            var coOApp = _mapper.Map<CoOApp>(dto);
            NormalizePersistedStringValues(coOApp);

            var now = DateTime.UtcNow;
            int currentUserId = 15;
            if (int.TryParse(_currentUser.UserId, out int id))
            {
                currentUserId = id;
            }

            var applicationOwnerId = applicantId ?? currentUserId;
            int? submittedById = applicantId.HasValue ? currentUserId : null;

            coOApp.CreatedAt = now;
            coOApp.CreatedBy = currentUserId;

            if (saveAsDraft)
            {
                NormalizeDraftValues(coOApp);
                await NormalizeDraftForeignKeysAsync(coOApp);
            }

            coOApp.Application = new Application
            {
                UserId = applicationOwnerId,
                SubmittedById = submittedById,
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
                coOApp.CoOAppReqDoc.ReqDocBldgPermitSPlans = await SaveFilesAsync(dto.CoOAppReqDoc.ReqDocBldgPermitSPlans, "req-docs");
                coOApp.CoOAppReqDoc.ReqDocAsBuiltPlans = await SaveFilesAsync(dto.CoOAppReqDoc.ReqDocAsBuiltPlans, "req-docs");
                coOApp.CoOAppReqDoc.ReqDocConsLogbook = await SaveFilesAsync(dto.CoOAppReqDoc.ReqDocConsLogbook, "req-docs");
                coOApp.CoOAppReqDoc.ReqDocConsPhotos = await SaveFilesAsync(dto.CoOAppReqDoc.ReqDocConsPhotos, "req-docs");
                coOApp.CoOAppReqDoc.ReqDocBrgyClearance = await SaveFilesAsync(dto.CoOAppReqDoc.ReqDocBrgyClearance, "req-docs");
                coOApp.CoOAppReqDoc.ReqDocFSIC = await SaveFilesAsync(dto.CoOAppReqDoc.ReqDocFSIC, "req-docs");

                if (dto.CoOAppReqDoc.ReqDocOthers != null)
                    coOApp.CoOAppReqDoc.ReqDocOthers = await SaveFilesAsync(dto.CoOAppReqDoc.ReqDocOthers, "req-docs");
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
            NormalizePersistedStringValues(coOApp);
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

            await UpdateMultiReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocBldgPermitSPlans), dto.CoOAppReqDoc.ReqDocBldgPermitSPlans, dto.CoOAppReqDoc.KeepReqDocBldgPermitSPlans, !saveAsDraft);
            await UpdateMultiReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocAsBuiltPlans), dto.CoOAppReqDoc.ReqDocAsBuiltPlans, dto.CoOAppReqDoc.KeepReqDocAsBuiltPlans, !saveAsDraft);
            await UpdateMultiReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocConsLogbook), dto.CoOAppReqDoc.ReqDocConsLogbook, dto.CoOAppReqDoc.KeepReqDocConsLogbook, !saveAsDraft);
            await UpdateMultiReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocConsPhotos), dto.CoOAppReqDoc.ReqDocConsPhotos, dto.CoOAppReqDoc.KeepReqDocConsPhotos, !saveAsDraft);
            await UpdateMultiReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocBrgyClearance), dto.CoOAppReqDoc.ReqDocBrgyClearance, dto.CoOAppReqDoc.KeepReqDocBrgyClearance, !saveAsDraft);
            await UpdateMultiReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocFSIC), dto.CoOAppReqDoc.ReqDocFSIC, dto.CoOAppReqDoc.KeepReqDocFSIC, !saveAsDraft);
            await UpdateMultiReqDocAsync(coOApp.CoOAppReqDoc, nameof(coOApp.CoOAppReqDoc.ReqDocOthers), dto.CoOAppReqDoc.ReqDocOthers, dto.CoOAppReqDoc.KeepReqDocOthers, false);

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

        private async Task<string> SaveFilesAsync(IFormFileCollection? files, string subFolder)
        {
            if (files == null || files.Count == 0)
                return string.Empty;

            var metadata = new List<FileMetadataDto>();
            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    continue;
                }

                var storedPath = await _fileStorageService.UploadAsync(file);
                if (!string.IsNullOrWhiteSpace(storedPath))
                {
                    metadata.Add(new FileMetadataDto
                    {
                        Name = file.FileName,
                        Size = file.Length,
                        Path = storedPath
                    });
                }
            }

            return FilePathHelper.Serialize(metadata);
        }

        private async Task UpdateMultiReqDocAsync(object target, string propertyName, IFormFileCollection? newFiles, string[] keepPaths, bool required)
        {
            var property = target.GetType().GetProperty(propertyName)!;
            var retained = FilePathHelper.Deserialize(property.GetValue(target) as string)
                .Where(file => keepPaths.Contains(file.Path, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (newFiles != null && newFiles.Count > 0)
            {
                retained.AddRange(await SaveNewFileMetadataAsync(newFiles));
            }

            if (required && retained.Count == 0)
            {
                throw new InvalidOperationException($"{propertyName} is required.");
            }

            property.SetValue(target, FilePathHelper.Serialize(retained));
        }

        private async Task<List<FileMetadataDto>> SaveNewFileMetadataAsync(IFormFileCollection files)
        {
            var metadata = new List<FileMetadataDto>();
            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    continue;
                }

                var storedPath = await _fileStorageService.UploadAsync(file);
                if (!string.IsNullOrWhiteSpace(storedPath))
                {
                    metadata.Add(new FileMetadataDto
                    {
                        Name = file.FileName,
                        Size = file.Length,
                        Path = storedPath
                    });
                }
            }

            return metadata;
        }

        private static void ValidateRequiredSubmission(CoOApp coOApp)
        {
            if (string.IsNullOrWhiteSpace(coOApp.BldgPermitNo)) throw new InvalidOperationException("Building permit number is required.");
            if (string.IsNullOrWhiteSpace(coOApp.ProjectTitle)) throw new InvalidOperationException("Project title is required.");
            if (string.IsNullOrWhiteSpace(coOApp.ProjLocLot)) throw new InvalidOperationException("Lot is required.");
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
            RequireSerializedFiles(coOApp.CoOAppReqDoc.ReqDocBldgPermitSPlans, "Approved building permit set of plans");
            RequireSerializedFiles(coOApp.CoOAppReqDoc.ReqDocAsBuiltPlans, "As-built plans");
            RequireSerializedFiles(coOApp.CoOAppReqDoc.ReqDocConsLogbook, "Construction logbook");
            RequireSerializedFiles(coOApp.CoOAppReqDoc.ReqDocConsPhotos, "Construction photos");
            RequireSerializedFiles(coOApp.CoOAppReqDoc.ReqDocBrgyClearance, "Barangay clearance");
            RequireSerializedFiles(coOApp.CoOAppReqDoc.ReqDocFSIC, "Fire safety inspection certificate");
        }

        private static void RequireSerializedFiles(string? value, string label)
        {
            if (string.IsNullOrWhiteSpace(value) || FilePathHelper.Deserialize(value).Count == 0)
            {
                throw new InvalidOperationException($"{label} is required.");
            }
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
            NormalizePersistedStringValues(coOApp);
            coOApp.CompletionDate = NormalizeDraftDate(coOApp.CompletionDate);
            coOApp.DateOfSignature = NormalizeDraftDate(coOApp.DateOfSignature);

            if (coOApp.CoOAppProf == null)
            {
                return;
            }

            coOApp.CoOAppProf.IOCValidity = NormalizeDraftDate(coOApp.CoOAppProf.IOCValidity);
            coOApp.CoOAppProf.EoRValidity = NormalizeDraftDate(coOApp.CoOAppProf.EoRValidity);
        }

        private static void NormalizePersistedStringValues(CoOApp coOApp)
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
            if (string.IsNullOrWhiteSpace(role)) return false;
            var govRoles = new[] { "admin", "superadmin", "sysadmin", "user", "encoder", "initial-reviewer", "technical-reviewer", "fee-assessor", "final-reviewer", "final-approver", "releasing-officer" };
            return govRoles.Any(r => string.Equals(role, r, StringComparison.OrdinalIgnoreCase));
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
