using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Entities.BuildingPermit;
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
    public class BuildingPermitService : IBuildingPermitService
    {
        private static readonly DateTime DraftPlaceholderDate = new(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly IBuildingPermitRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepository _userRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IEmailService _emailService;
        private readonly IAdminEmailNotificationConfigService _adminEmailNotificationConfigService;
        private readonly IApplicationFormattedIdService _applicationFormattedIdService;
        private readonly ILogger<BuildingPermitService> _logger;
        private readonly ApplicationDbContext _dbContext;

        public BuildingPermitService(
            IBuildingPermitRepository repository,
            IMapper mapper,
            ICurrentUserService currentUser,
            IUserRepository userRepository,
            IFileStorageService fileStorageService,
            IEmailService emailService,
            IAdminEmailNotificationConfigService adminEmailNotificationConfigService,
            IApplicationFormattedIdService applicationFormattedIdService,
            ApplicationDbContext dbContext,
            ILogger<BuildingPermitService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userRepository = userRepository;
            _fileStorageService = fileStorageService;
            _emailService = emailService;
            _adminEmailNotificationConfigService = adminEmailNotificationConfigService;
            _applicationFormattedIdService = applicationFormattedIdService;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<PagedResult<BuildingPermit>> GetAllAsync(PaginationParams pagination)
        {
            return await _repository.GetAllAsync(pagination);
        }

        public async Task<BuildingPermit?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<BuildingPermitEditDto?> GetEditByApplicationIdAsync(int applicationId)
        {
            var buildingPermit = await _repository.GetByApplicationIdAsync(applicationId);
            if (!await CanEditAsync(buildingPermit?.Application))
            {
                return null;
            }

            return buildingPermit == null ? null : _mapper.Map<BuildingPermitEditDto>(buildingPermit);
        }

        public async Task<BuildingPermitEditDto?> GetFormByApplicationIdAsync(int applicationId)
        {
            var buildingPermit = await _repository.GetByApplicationIdAsync(applicationId);
            if (!await CanViewFormAsync(buildingPermit?.Application))
            {
                return null;
            }

            return buildingPermit == null ? null : _mapper.Map<BuildingPermitEditDto>(buildingPermit);
        }

        public async Task<BuildingPermit> CreateAsync(BuildingPermitCreateDto dto, bool saveAsDraft = false, int? applicantId = null)
        {
            var buildingPermit = _mapper.Map<BuildingPermit>(dto);

            var now = DateTime.UtcNow;
            int currentUserId = 15;
            if (int.TryParse(_currentUser.UserId, out int id))
            {
                currentUserId = id;
            }

            // If encoder submits on behalf of applicant, use applicantId as owner
            var applicationOwnerId = applicantId ?? currentUserId;
            int? submittedById = applicantId.HasValue ? currentUserId : null;

            buildingPermit.CreatedAt = now;
            buildingPermit.CreatedBy = currentUserId;

            if (saveAsDraft)
            {
                NormalizeDraftValues(buildingPermit);
                await NormalizeDraftForeignKeysAsync(buildingPermit);
            }

            buildingPermit.Application = new Application
            {
                UserId = applicationOwnerId,
                SubmittedById = submittedById,
                Type = ApplicationWorkflowDefinitions.PermitTypes.BuildingPermit,
                Status = saveAsDraft
                    ? ApplicationWorkflowDefinitions.OverallStatuses.Draft
                    : ApplicationWorkflowDefinitions.OverallStatuses.Submitted,
                CreatedAt = now,
                CreatedBy = _currentUser.UserName ?? "System",
                DepartmentReviews = saveAsDraft
                    ? new List<ApplicationDepartmentReview>()
                    : ApplicationWorkflowDefinitions
                        .GetRequiredDepartmentIds(ApplicationWorkflowDefinitions.PermitTypes.BuildingPermit)
                        .Select(departmentId => new ApplicationDepartmentReview
                        {
                            DepartmentId = departmentId,
                            Status = ApplicationWorkflowDefinitions.DepartmentStatuses.InQueue,
                            CreatedAt = now
                        })
                        .ToList()
            };

            if (buildingPermit.AppInfo != null)
            {
                buildingPermit.AppInfo.CreatedAt = now;
                buildingPermit.AppInfo.CreatedBy = currentUserId;
            }

            if (buildingPermit.DesignProf != null)
            {
                buildingPermit.DesignProf.CreatedAt = now;
                buildingPermit.DesignProf.CreatedBy = currentUserId;
            }

            if (buildingPermit.TechDoc != null)
            {
                buildingPermit.TechDoc.CreatedAt = now;
                buildingPermit.TechDoc.CreatedBy = currentUserId;
            }

            if (buildingPermit.SupportingDoc != null)
            {
                buildingPermit.SupportingDoc.CreatedAt = now;
                buildingPermit.SupportingDoc.CreatedBy = currentUserId;
            }
            
            await _repository.AddAsync(buildingPermit);
            await _repository.SaveChangesAsync();

            if (!saveAsDraft)
            {
                await _applicationFormattedIdService.AssignFormattedIdAsync(buildingPermit.Application);
            }

            // Save files
            if (buildingPermit.AppInfo != null)
            {
                buildingPermit.AppInfo.ReqDocProofOwnership = await SaveFilesAsync(dto.AppInfo.ReqDocProofOwnership, buildingPermit.Id, "req-docs");
                buildingPermit.AppInfo.ReqDocBarangayClearance = await SaveFilesAsync(dto.AppInfo.ReqDocBarangayClearance, buildingPermit.Id, "req-docs");
                buildingPermit.AppInfo.ReqDocTaxDeclaration = await SaveFilesAsync(dto.AppInfo.ReqDocTaxDeclaration, buildingPermit.Id, "req-docs");
                buildingPermit.AppInfo.ReqDocRealPropTaxReceipt = await SaveFilesAsync(dto.AppInfo.ReqDocRealPropTaxReceipt, buildingPermit.Id, "req-docs");

                if (dto.AppInfo.ReqDocECCorCNC != null)
                    buildingPermit.AppInfo.ReqDocECCorCNC = await SaveFilesAsync(dto.AppInfo.ReqDocECCorCNC, buildingPermit.Id, "req-docs");

                if (dto.AppInfo.ReqDocSpecialClearances != null)
                    buildingPermit.AppInfo.ReqDocSpecialClearances = await SaveFilesAsync(dto.AppInfo.ReqDocSpecialClearances, buildingPermit.Id, "req-docs");
            }

            if (buildingPermit.TechDoc != null)
            {
                buildingPermit.TechDoc.TechDocIoCPlans = await SaveFilesAsync(dto.TechDoc.TechDocIoCPlans, buildingPermit.Id, "tech-docs");
                buildingPermit.TechDoc.TechDocSEPlans = await SaveFilesAsync(dto.TechDoc.TechDocSEPlans, buildingPermit.Id, "tech-docs");
                buildingPermit.TechDoc.TechDocEEPlans = await SaveFilesAsync(dto.TechDoc.TechDocEEPlans, buildingPermit.Id, "tech-docs");
                buildingPermit.TechDoc.TechDocSPPlans = await SaveFilesAsync(dto.TechDoc.TechDocSPPlans, buildingPermit.Id, "tech-docs");
                if (dto.TechDoc.TechDocStructuralAnalysisDesign != null)
                    buildingPermit.TechDoc.TechDocStructuralAnalysisDesign = await SaveFilesAsync(dto.TechDoc.TechDocStructuralAnalysisDesign, buildingPermit.Id, "tech-docs");
                if (dto.TechDoc.TechDocFireSafetyPlans != null)
                    buildingPermit.TechDoc.TechDocFireSafetyPlans = await SaveFilesAsync(dto.TechDoc.TechDocFireSafetyPlans, buildingPermit.Id, "tech-docs");
                if (dto.TechDoc.TechDocEnvironmentalDocuments != null)
                    buildingPermit.TechDoc.TechDocEnvironmentalDocuments = await SaveFilesAsync(dto.TechDoc.TechDocEnvironmentalDocuments, buildingPermit.Id, "tech-docs");
                if (dto.TechDoc.TechDocSoilTestFieldDensityTest != null)
                    buildingPermit.TechDoc.TechDocSoilTestFieldDensityTest = await SaveFilesAsync(dto.TechDoc.TechDocSoilTestFieldDensityTest, buildingPermit.Id, "tech-docs");
                buildingPermit.TechDoc.TechDocBOMCost = await SaveFilesAsync(dto.TechDoc.TechDocBOMCost, buildingPermit.Id, "tech-docs");
                buildingPermit.TechDoc.TechDocSoW = await SaveFilesAsync(dto.TechDoc.TechDocSoW, buildingPermit.Id, "tech-docs");

                if (dto.TechDoc.TechDocMEPlans != null)
                    buildingPermit.TechDoc.TechDocMEPlans = await SaveFilesAsync(dto.TechDoc.TechDocMEPlans, buildingPermit.Id, "tech-docs");

                if (dto.TechDoc.TechDocECEPlans != null)
                    buildingPermit.TechDoc.TechDocECEPlans = await SaveFilesAsync(dto.TechDoc.TechDocECEPlans, buildingPermit.Id, "tech-docs");
            }

            if (buildingPermit.SupportingDoc != null)
            {
                await UpdateSupportingDocFilesAsync(buildingPermit.SupportingDoc, dto.SupportingDoc, buildingPermit.Id);
            }

            if (!saveAsDraft)
            {
                ValidateRequiredSubmission(buildingPermit);
                ValidateCategorySpecificRequirements(buildingPermit);
            }

            // Update again with file paths
            _repository.Update(buildingPermit);
            if (buildingPermit.AppInfo != null) _repository.Update(buildingPermit); // This might be redundant but ensuring graph is marked
            await _repository.SaveChangesAsync();

            // Send notification emails
            if (!saveAsDraft)
            {
                var applicantName = buildingPermit.AppInfo?.FullName ?? "Unknown Applicant";
                await SendAdminNotificationEmailsAsync(
                    buildingPermit.Application,
                    applicantName,
                    "Building Permit");
                await SendApplicantSubmissionEmailAsync(
                    buildingPermit.AppInfo?.Email,
                    applicantName,
                    buildingPermit.Application,
                    "Building Permit");
            }

            return buildingPermit;
        }

        public async Task<(bool Success, string Message, BuildingPermit? BuildingPermit)> UpdateByApplicationIdAsync(int applicationId, BuildingPermitUpdateDto dto, bool saveAsDraft = false)
        {
            var buildingPermit = await _repository.GetByApplicationIdAsync(applicationId);
            if (buildingPermit?.Application == null || buildingPermit.AppInfo == null || buildingPermit.DesignProf == null || buildingPermit.TechDoc == null)
            {
                return (false, "Application not found", null);
            }

            if (!await CanEditAsync(buildingPermit.Application))
            {
                return (false, "This application can no longer be edited", null);
            }

            var now = DateTime.UtcNow;
            var actor = _currentUser.UserName ?? "System";
            var currentUserId = TryGetCurrentUserId();
            var wasDraft = string.Equals(
                buildingPermit.Application.Status,
                ApplicationWorkflowDefinitions.OverallStatuses.Draft,
                StringComparison.OrdinalIgnoreCase);

            UpdateBuildingPermitFields(buildingPermit, dto, now, currentUserId);
            UpdateAppInfoFields(buildingPermit.AppInfo, dto.AppInfo, now, currentUserId);
            _mapper.Map(dto.DesignProf, buildingPermit.DesignProf);
            buildingPermit.DesignProf.UpdatedAt = now;
            buildingPermit.DesignProf.UpdatedBy = currentUserId;

            if (buildingPermit.SupportingDoc == null)
            {
                buildingPermit.SupportingDoc = new BuildingPermitSupportingDoc
                {
                    CreatedAt = now,
                    CreatedBy = currentUserId
                };
            }

            buildingPermit.SupportingDoc.UpdatedAt = now;
            buildingPermit.SupportingDoc.UpdatedBy = currentUserId;

            if (saveAsDraft)
            {
                NormalizeDraftValues(buildingPermit);
                await NormalizeDraftForeignKeysAsync(buildingPermit);
            }

            await UpdateMultiFileAsync(buildingPermit.AppInfo, nameof(buildingPermit.AppInfo.ReqDocProofOwnership), dto.AppInfo.KeepReqDocProofOwnership, dto.AppInfo.ReqDocProofOwnership, buildingPermit.Id, !saveAsDraft);
            await UpdateMultiFileAsync(buildingPermit.AppInfo, nameof(buildingPermit.AppInfo.ReqDocBarangayClearance), dto.AppInfo.KeepReqDocBarangayClearance, dto.AppInfo.ReqDocBarangayClearance, buildingPermit.Id, !saveAsDraft);
            await UpdateMultiFileAsync(buildingPermit.AppInfo, nameof(buildingPermit.AppInfo.ReqDocTaxDeclaration), dto.AppInfo.KeepReqDocTaxDeclaration, dto.AppInfo.ReqDocTaxDeclaration, buildingPermit.Id, !saveAsDraft);
            await UpdateMultiFileAsync(buildingPermit.AppInfo, nameof(buildingPermit.AppInfo.ReqDocRealPropTaxReceipt), dto.AppInfo.KeepReqDocRealPropTaxReceipt, dto.AppInfo.ReqDocRealPropTaxReceipt, buildingPermit.Id, !saveAsDraft);
            await UpdateMultiFileAsync(buildingPermit.AppInfo, nameof(buildingPermit.AppInfo.ReqDocECCorCNC), dto.AppInfo.KeepReqDocECCorCNC, dto.AppInfo.ReqDocECCorCNC, buildingPermit.Id, false);
            await UpdateMultiFileAsync(buildingPermit.AppInfo, nameof(buildingPermit.AppInfo.ReqDocSpecialClearances), dto.AppInfo.KeepReqDocSpecialClearances, dto.AppInfo.ReqDocSpecialClearances, buildingPermit.Id, false);

            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocIoCPlans), dto.TechDoc.KeepTechDocIoCPlans, dto.TechDoc.TechDocIoCPlans, buildingPermit.Id, !saveAsDraft);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocSEPlans), dto.TechDoc.KeepTechDocSEPlans, dto.TechDoc.TechDocSEPlans, buildingPermit.Id, !saveAsDraft);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocEEPlans), dto.TechDoc.KeepTechDocEEPlans, dto.TechDoc.TechDocEEPlans, buildingPermit.Id, !saveAsDraft);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocSPPlans), dto.TechDoc.KeepTechDocSPPlans, dto.TechDoc.TechDocSPPlans, buildingPermit.Id, !saveAsDraft);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocStructuralAnalysisDesign), dto.TechDoc.KeepTechDocStructuralAnalysisDesign, dto.TechDoc.TechDocStructuralAnalysisDesign, buildingPermit.Id, false);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocFireSafetyPlans), dto.TechDoc.KeepTechDocFireSafetyPlans, dto.TechDoc.TechDocFireSafetyPlans, buildingPermit.Id, false);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocEnvironmentalDocuments), dto.TechDoc.KeepTechDocEnvironmentalDocuments, dto.TechDoc.TechDocEnvironmentalDocuments, buildingPermit.Id, false);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocSoilTestFieldDensityTest), dto.TechDoc.KeepTechDocSoilTestFieldDensityTest, dto.TechDoc.TechDocSoilTestFieldDensityTest, buildingPermit.Id, false);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocBOMCost), dto.TechDoc.KeepTechDocBOMCost, dto.TechDoc.TechDocBOMCost, buildingPermit.Id, !saveAsDraft);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocSoW), dto.TechDoc.KeepTechDocSoW, dto.TechDoc.TechDocSoW, buildingPermit.Id, !saveAsDraft);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocMEPlans), dto.TechDoc.KeepTechDocMEPlans, dto.TechDoc.TechDocMEPlans, buildingPermit.Id, false);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocECEPlans), dto.TechDoc.KeepTechDocECEPlans, dto.TechDoc.TechDocECEPlans, buildingPermit.Id, false);

            await UpdateMultiFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocZoningClearance), dto.SupportingDoc.KeepSupportDocZoningClearance, dto.SupportingDoc.SupportDocZoningClearance, buildingPermit.Id, false, "supporting-docs");
            await UpdateMultiFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocLocationalClearance), dto.SupportingDoc.KeepSupportDocLocationalClearance, dto.SupportingDoc.SupportDocLocationalClearance, buildingPermit.Id, false, "supporting-docs");
            await UpdateMultiFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocFireSafetyClearance), dto.SupportingDoc.KeepSupportDocFireSafetyClearance, dto.SupportingDoc.SupportDocFireSafetyClearance, buildingPermit.Id, false, "supporting-docs");
            await UpdateMultiFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocHighwayClearance), dto.SupportingDoc.KeepSupportDocHighwayClearance, dto.SupportingDoc.SupportDocHighwayClearance, buildingPermit.Id, false, "supporting-docs");
            await UpdateMultiFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocHeightClearance), dto.SupportingDoc.KeepSupportDocHeightClearance, dto.SupportingDoc.SupportDocHeightClearance, buildingPermit.Id, false, "supporting-docs");
            await UpdateMultiFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocECCorCNC), dto.SupportingDoc.KeepSupportDocECCorCNC, dto.SupportingDoc.SupportDocECCorCNC, buildingPermit.Id, false, "supporting-docs");
            await UpdateMultiFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocDENRClearance), dto.SupportingDoc.KeepSupportDocDENRClearance, dto.SupportingDoc.SupportDocDENRClearance, buildingPermit.Id, false, "supporting-docs");
            await UpdateMultiFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocSECRegistration), dto.SupportingDoc.KeepSupportDocSECRegistration, dto.SupportingDoc.SupportDocSECRegistration, buildingPermit.Id, false, "supporting-docs");
            await UpdateMultiFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocBoardResolution), dto.SupportingDoc.KeepSupportDocBoardResolution, dto.SupportingDoc.SupportDocBoardResolution, buildingPermit.Id, false, "supporting-docs");
            await UpdateMultiFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocHOAClearance), dto.SupportingDoc.KeepSupportDocHOAClearance, dto.SupportingDoc.SupportDocHOAClearance, buildingPermit.Id, false, "supporting-docs");

            if (saveAsDraft)
            {
                buildingPermit.Application.Status = ApplicationWorkflowDefinitions.OverallStatuses.Draft;
                buildingPermit.Application.FormattedId = string.Empty;
                buildingPermit.Application.DepartmentReviews.Clear();
            }
            else
            {
                EnsureSubmittedState(buildingPermit.Application, now);
                ValidateRequiredSubmission(buildingPermit);
                ValidateCategorySpecificRequirements(buildingPermit);

                if (wasDraft)
                {
                    buildingPermit.Application.CreatedAt = now;
                    await _applicationFormattedIdService.AssignFormattedIdAsync(buildingPermit.Application);
                }
            }

            buildingPermit.Application.UpdatedAt = now;
            buildingPermit.Application.UpdatedBy = actor;

            _repository.Update(buildingPermit);
            await _repository.SaveChangesAsync();

            if (!saveAsDraft && wasDraft)
            {
                var applicantName = buildingPermit.AppInfo?.FullName ?? "Unknown Applicant";
                await SendAdminNotificationEmailsAsync(
                    buildingPermit.Application,
                    applicantName,
                    "Building Permit");
                await SendApplicantSubmissionEmailAsync(
                    buildingPermit.AppInfo?.Email,
                    applicantName,
                    buildingPermit.Application,
                    "Building Permit");
            }

            return (true, saveAsDraft ? "Draft saved successfully" : "Application updated successfully", buildingPermit);
        }

        private async Task<string> SaveFilesAsync(IFormFileCollection? files, int permitId, string subFolder)
        {
            if (files == null || files.Count == 0)
                return string.Empty;

            var metadataList = new List<FileMetadataDto>();

            foreach (var file in files)
            {
                var storedFileName = await SaveFileInternalAsync(file);
                if (!string.IsNullOrEmpty(storedFileName))
                {
                    metadataList.Add(new FileMetadataDto
                    {
                        Name = file.FileName,
                        Size = file.Length,
                        Path = storedFileName
                    });
                }
            }

            return FilePathHelper.Serialize(metadataList);
        }

        private async Task<string> SaveFileInternalAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            return await _fileStorageService.UploadAsync(file);
        }

        private static void UpdateBuildingPermitFields(BuildingPermit buildingPermit, BuildingPermitUpdateDto dto, DateTime now, int currentUserId)
        {
            buildingPermit.PermitAppTypeId = dto.PermitAppTypeId;
            buildingPermit.BuildingPermitCategoryId = dto.BuildingPermitCategoryId;
            buildingPermit.OccupancyNatureId = dto.OccupancyNatureId;
            buildingPermit.ProjectTitle = dto.ProjectTitle;
            buildingPermit.ProjectClassId = dto.ProjectClassId;
            buildingPermit.EstimatedCost = dto.EstimatedCost;
            buildingPermit.NoOfStoreys = dto.NoOfStoreys;
            buildingPermit.FloorAreaPerStorey = dto.FloorAreaPerStorey;
            buildingPermit.TotalFloorArea = dto.TotalFloorArea;
            buildingPermit.ProjectScopeLotArea = dto.ProjectScopeLotArea;
            buildingPermit.PropertyAddBlock = dto.PropertyAddBlock;
            buildingPermit.PropertyAddLot = dto.PropertyAddLot;
            buildingPermit.PropertyAddStreet = dto.PropertyAddStreet;
            buildingPermit.ProvinceId = dto.ProvinceId;
            buildingPermit.LGUId = dto.LGUId;
            buildingPermit.BarangayId = dto.BarangayId;
            buildingPermit.PropertyDetailLotArea = dto.PropertyDetailLotArea;
            buildingPermit.TCTNo = dto.TCTNo;
            buildingPermit.TaxDeclarionNo = dto.TaxDeclarionNo;
            buildingPermit.Coordinates = dto.Coordinates;
            buildingPermit.Accessories = string.Join("|", dto.Accessories);
            buildingPermit.DigitalSignature = dto.DigitalSignature;
            buildingPermit.DateofSignature = dto.DateofSignature ?? default;
            buildingPermit.UpdatedAt = now;
            buildingPermit.UpdatedBy = currentUserId;
        }

        private static void UpdateAppInfoFields(BuildingPermitAppInfo appInfo, BuildingPermitAppInfoUpdateDto dto, DateTime now, int currentUserId)
        {
            appInfo.ApplicantTypeId = dto.ApplicantTypeId;
            appInfo.FullName = dto.FullName;
            appInfo.ContactNo = dto.ContactNo;
            appInfo.Email = dto.Email;
            appInfo.TIN = dto.TIN;
            appInfo.MailAddress = dto.MailAddress;
            appInfo.OwnershipTypeId = dto.OwnershipTypeId;
            appInfo.UpdatedAt = now;
            appInfo.UpdatedBy = currentUserId;
        }

        private async Task UpdateSupportingDocFilesAsync(BuildingPermitSupportingDoc supportingDoc, BuildingPermitSupportingDocCreateDto dto, int permitId)
        {
            supportingDoc.SupportDocZoningClearance = await SaveFilesAsync(dto.SupportDocZoningClearance, permitId, "supporting-docs");
            supportingDoc.SupportDocLocationalClearance = await SaveFilesAsync(dto.SupportDocLocationalClearance, permitId, "supporting-docs");
            supportingDoc.SupportDocFireSafetyClearance = await SaveFilesAsync(dto.SupportDocFireSafetyClearance, permitId, "supporting-docs");
            supportingDoc.SupportDocHighwayClearance = await SaveFilesAsync(dto.SupportDocHighwayClearance, permitId, "supporting-docs");
            supportingDoc.SupportDocHeightClearance = await SaveFilesAsync(dto.SupportDocHeightClearance, permitId, "supporting-docs");
            supportingDoc.SupportDocECCorCNC = await SaveFilesAsync(dto.SupportDocECCorCNC, permitId, "supporting-docs");
            supportingDoc.SupportDocDENRClearance = await SaveFilesAsync(dto.SupportDocDENRClearance, permitId, "supporting-docs");
            supportingDoc.SupportDocSECRegistration = await SaveFilesAsync(dto.SupportDocSECRegistration, permitId, "supporting-docs");
            supportingDoc.SupportDocBoardResolution = await SaveFilesAsync(dto.SupportDocBoardResolution, permitId, "supporting-docs");
            supportingDoc.SupportDocHOAClearance = await SaveFilesAsync(dto.SupportDocHOAClearance, permitId, "supporting-docs");
        }

        private async Task UpdateMultiFileAsync(object target, string propertyName, string[] keepPaths, IFormFileCollection? newFiles, int permitId, bool required, string subFolder = "tech-docs")
        {
            var property = target.GetType().GetProperty(propertyName)!;
            var existing = FilePathHelper.Deserialize(property.GetValue(target) as string);
            var retained = existing.Where(file => keepPaths.Contains(file.Path, StringComparer.OrdinalIgnoreCase)).ToList();

            if (newFiles != null && newFiles.Count > 0)
            {
                retained.AddRange(await SaveNewFileMetadataAsync(newFiles, permitId, subFolder));
            }

            if (required && retained.Count == 0)
            {
                throw new InvalidOperationException($"{propertyName} is required.");
            }

            property.SetValue(target, FilePathHelper.Serialize(retained));
        }

        private async Task<List<FileMetadataDto>> SaveNewFileMetadataAsync(IFormFileCollection files, int permitId, string subFolder)
        {
            var result = new List<FileMetadataDto>();
            foreach (var file in files)
            {
                var storedFileName = await SaveFileInternalAsync(file);
                if (!string.IsNullOrWhiteSpace(storedFileName))
                {
                    result.Add(new FileMetadataDto
                    {
                        Name = file.FileName,
                        Size = file.Length,
                        Path = storedFileName
                    });
                }
            }

            return result;
        }

        private static readonly string[] ComplexSupportingDocFields =
        {
            nameof(BuildingPermitSupportingDoc.SupportDocZoningClearance),
            nameof(BuildingPermitSupportingDoc.SupportDocLocationalClearance),
            nameof(BuildingPermitSupportingDoc.SupportDocFireSafetyClearance),
            nameof(BuildingPermitSupportingDoc.SupportDocHighwayClearance),
            nameof(BuildingPermitSupportingDoc.SupportDocHeightClearance),
            nameof(BuildingPermitSupportingDoc.SupportDocECCorCNC),
            nameof(BuildingPermitSupportingDoc.SupportDocDENRClearance),
        };

        private static readonly string[] SimpleSupportingDocFields =
        {
            nameof(BuildingPermitSupportingDoc.SupportDocZoningClearance),
            nameof(BuildingPermitSupportingDoc.SupportDocLocationalClearance),
            nameof(BuildingPermitSupportingDoc.SupportDocFireSafetyClearance),
        };

        private static void ValidateRequiredSubmission(BuildingPermit buildingPermit)
        {
            if (buildingPermit.PermitAppTypeId <= 0) throw new InvalidOperationException("Application type is required.");
            if (buildingPermit.BuildingPermitCategoryId <= 0) throw new InvalidOperationException("Building permit category is required.");
            if (buildingPermit.OccupancyNatureId <= 0) throw new InvalidOperationException("Occupancy nature is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.ProjectTitle)) throw new InvalidOperationException("Project title is required.");
            if (buildingPermit.ProjectClassId <= 0) throw new InvalidOperationException("Project classification is required.");
            if (buildingPermit.EstimatedCost <= 0) throw new InvalidOperationException("Estimated cost is required.");
            if (buildingPermit.NoOfStoreys <= 0) throw new InvalidOperationException("Number of storeys is required.");
            if (buildingPermit.FloorAreaPerStorey <= 0) throw new InvalidOperationException("Floor area per storey is required.");
            if (buildingPermit.TotalFloorArea <= 0) throw new InvalidOperationException("Total floor area is required.");
            if (buildingPermit.ProjectScopeLotArea <= 0) throw new InvalidOperationException("Project scope lot area is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.PropertyAddLot)) throw new InvalidOperationException("Lot is required.");
            if (buildingPermit.ProvinceId <= 0) throw new InvalidOperationException("Province is required.");
            if (buildingPermit.LGUId <= 0) throw new InvalidOperationException("LGU is required.");
            if (buildingPermit.BarangayId <= 0) throw new InvalidOperationException("Barangay is required.");
            if (buildingPermit.PropertyDetailLotArea <= 0) throw new InvalidOperationException("Property lot area is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.TCTNo)) throw new InvalidOperationException("TCT number is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.DigitalSignature)) throw new InvalidOperationException("Digital signature is required.");
            if (IsDraftPlaceholderDate(buildingPermit.DateofSignature)) throw new InvalidOperationException("Signature date is required.");

            if (buildingPermit.AppInfo == null) throw new InvalidOperationException("Applicant information is required.");
            if (buildingPermit.DesignProf == null) throw new InvalidOperationException("Design professionals are required.");
            if (buildingPermit.TechDoc == null) throw new InvalidOperationException("Technical documents are required.");
            if (buildingPermit.SupportingDoc == null) throw new InvalidOperationException("Supporting documents are required.");

            if (buildingPermit.AppInfo.ApplicantTypeId <= 0) throw new InvalidOperationException("Applicant type is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.AppInfo.FullName)) throw new InvalidOperationException("Applicant full name is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.AppInfo.ContactNo)) throw new InvalidOperationException("Applicant contact number is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.AppInfo.Email)) throw new InvalidOperationException("Applicant email is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.AppInfo.TIN)) throw new InvalidOperationException("TIN is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.AppInfo.MailAddress)) throw new InvalidOperationException("Mailing address is required.");
            if (buildingPermit.AppInfo.OwnershipTypeId <= 0) throw new InvalidOperationException("Ownership type is required.");
            RequireSerializedFiles(buildingPermit.AppInfo.ReqDocProofOwnership, "Proof of ownership");
            RequireSerializedFiles(buildingPermit.AppInfo.ReqDocBarangayClearance, "Barangay clearance");
            RequireSerializedFiles(buildingPermit.AppInfo.ReqDocTaxDeclaration, "Tax declaration");
            RequireSerializedFiles(buildingPermit.AppInfo.ReqDocRealPropTaxReceipt, "Real property tax receipt");

            if (string.IsNullOrWhiteSpace(buildingPermit.DesignProf.IoCFullName)) throw new InvalidOperationException("Architect / Civil Engineer is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.DesignProf.IoCPRCNo)) throw new InvalidOperationException("Architect / Civil Engineer PRC number is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.DesignProf.IoCPTRNo)) throw new InvalidOperationException("Architect / Civil Engineer PTR number is required.");
            if (IsDraftPlaceholderDate(buildingPermit.DesignProf.IOCValidity)) throw new InvalidOperationException("Architect / Civil Engineer validity is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.DesignProf.SEFullName)) throw new InvalidOperationException("Structural engineer is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.DesignProf.SEPRCNo)) throw new InvalidOperationException("Structural engineer PRC number is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.DesignProf.SEPTRNo)) throw new InvalidOperationException("Structural engineer PTR number is required.");
            if (IsDraftPlaceholderDate(buildingPermit.DesignProf.SEValidity)) throw new InvalidOperationException("Structural engineer validity is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.DesignProf.EEFullName)) throw new InvalidOperationException("Electrical engineer is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.DesignProf.EEPRCNo)) throw new InvalidOperationException("Electrical engineer PRC number is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.DesignProf.EEPTRNo)) throw new InvalidOperationException("Electrical engineer PTR number is required.");
            if (IsDraftPlaceholderDate(buildingPermit.DesignProf.EEValidity)) throw new InvalidOperationException("Electrical engineer validity is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.DesignProf.SPEFullName)) throw new InvalidOperationException("Sanitary / plumbing engineer is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.DesignProf.SPEPRCNo)) throw new InvalidOperationException("Sanitary / plumbing engineer PRC number is required.");
            if (string.IsNullOrWhiteSpace(buildingPermit.DesignProf.SPEPTRNo)) throw new InvalidOperationException("Sanitary / plumbing engineer PTR number is required.");
            if (IsDraftPlaceholderDate(buildingPermit.DesignProf.SPEValidity)) throw new InvalidOperationException("Sanitary / plumbing engineer validity is required.");

            RequireSerializedFiles(buildingPermit.TechDoc.TechDocIoCPlans, "Architectural plans");
            RequireSerializedFiles(buildingPermit.TechDoc.TechDocSEPlans, "Structural plans");
            RequireSerializedFiles(buildingPermit.TechDoc.TechDocEEPlans, "Electrical plans");
            RequireSerializedFiles(buildingPermit.TechDoc.TechDocSPPlans, "Sanitary plans");
            RequireSerializedFiles(buildingPermit.TechDoc.TechDocBOMCost, "Bill of materials");
            RequireSerializedFiles(buildingPermit.TechDoc.TechDocSoW, "Specifications");
        }

        private static void ValidateCategorySpecificRequirements(BuildingPermit buildingPermit)
        {
            var categoryId = buildingPermit.BuildingPermitCategoryId;
            var isComplex = categoryId == 2;
            var isHighlyTechnical = categoryId == 3;
            var designProf = buildingPermit.DesignProf;
            var techDoc = buildingPermit.TechDoc;
            var supportingDoc = buildingPermit.SupportingDoc;

            if (designProf == null || techDoc == null || supportingDoc == null)
            {
                throw new InvalidOperationException("Application data is incomplete.");
            }

            if (isHighlyTechnical && string.IsNullOrWhiteSpace(designProf.MEFullName))
            {
                throw new InvalidOperationException("Mechanical Engineer is required.");
            }

            if (isHighlyTechnical && string.IsNullOrWhiteSpace(designProf.GSEFullName))
            {
                throw new InvalidOperationException("Geotechnical / Soil Engineer is required.");
            }

            if (isComplex || isHighlyTechnical)
            {
                RequireSerializedFiles(techDoc.TechDocStructuralAnalysisDesign, "Structural Analysis & Design");
                RequireSerializedFiles(techDoc.TechDocFireSafetyPlans, "Fire Safety Plans");
                RequireSerializedFiles(techDoc.TechDocEnvironmentalDocuments, "Environmental Documents");
            }

            if (isHighlyTechnical)
            {
                RequireSerializedFiles(techDoc.TechDocSoilTestFieldDensityTest, "Soil Test / Field Density Test");
            }

            var requiredSupportingDocs = isComplex || isHighlyTechnical
                ? ComplexSupportingDocFields
                : SimpleSupportingDocFields;

            foreach (var field in requiredSupportingDocs)
            {
                var value = supportingDoc.GetType().GetProperty(field)?.GetValue(supportingDoc) as string;
                RequireSerializedFiles(value, field);
            }

            if (buildingPermit.AppInfo?.ApplicantTypeId == 2 && FilePathHelper.Deserialize(supportingDoc.SupportDocSECRegistration).Count == 0)
            {
                throw new InvalidOperationException("SupportDocSECRegistration is required.");
            }
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
                .GetRequiredDepartmentIds(ApplicationWorkflowDefinitions.PermitTypes.BuildingPermit)
                .Select(departmentId => new ApplicationDepartmentReview
                {
                    DepartmentId = departmentId,
                    Status = ApplicationWorkflowDefinitions.DepartmentStatuses.InQueue,
                    CreatedAt = now
                })
                .ToList();
        }

        private static void NormalizeDraftValues(BuildingPermit buildingPermit)
        {
            buildingPermit.ProjectTitle ??= string.Empty;
            buildingPermit.PropertyAddLot ??= string.Empty;
            buildingPermit.PropertyAddStreet ??= string.Empty;
            buildingPermit.TCTNo ??= string.Empty;
            buildingPermit.TaxDeclarionNo ??= string.Empty;
            buildingPermit.DigitalSignature ??= string.Empty;
            buildingPermit.DateofSignature = NormalizeDraftDate(buildingPermit.DateofSignature);

            if (buildingPermit.AppInfo != null)
            {
                buildingPermit.AppInfo.FullName ??= string.Empty;
                buildingPermit.AppInfo.ContactNo ??= string.Empty;
                buildingPermit.AppInfo.Email ??= string.Empty;
                buildingPermit.AppInfo.TIN ??= string.Empty;
                buildingPermit.AppInfo.MailAddress ??= string.Empty;
            }

            if (buildingPermit.DesignProf != null)
            {
                buildingPermit.DesignProf.IoCFullName ??= string.Empty;
                buildingPermit.DesignProf.IoCPRCNo ??= string.Empty;
                buildingPermit.DesignProf.IoCPTRNo ??= string.Empty;
                buildingPermit.DesignProf.IOCValidity = NormalizeDraftDate(buildingPermit.DesignProf.IOCValidity);
                buildingPermit.DesignProf.SEFullName ??= string.Empty;
                buildingPermit.DesignProf.SEPRCNo ??= string.Empty;
                buildingPermit.DesignProf.SEPTRNo ??= string.Empty;
                buildingPermit.DesignProf.SEValidity = NormalizeDraftDate(buildingPermit.DesignProf.SEValidity);
                buildingPermit.DesignProf.EEFullName ??= string.Empty;
                buildingPermit.DesignProf.EEPRCNo ??= string.Empty;
                buildingPermit.DesignProf.EEPTRNo ??= string.Empty;
                buildingPermit.DesignProf.EEValidity = NormalizeDraftDate(buildingPermit.DesignProf.EEValidity);
                buildingPermit.DesignProf.SPEFullName ??= string.Empty;
                buildingPermit.DesignProf.SPEPRCNo ??= string.Empty;
                buildingPermit.DesignProf.SPEPTRNo ??= string.Empty;
                buildingPermit.DesignProf.SPEValidity = NormalizeDraftDate(buildingPermit.DesignProf.SPEValidity);
            }
        }

        private static DateTime NormalizeDraftDate(DateTime date)
        {
            return date == default ? DraftPlaceholderDate : date;
        }

        private static bool IsDraftPlaceholderDate(DateTime date)
        {
            return date == default || date == DraftPlaceholderDate;
        }

        private async Task NormalizeDraftForeignKeysAsync(BuildingPermit buildingPermit)
        {
            if (buildingPermit.PermitAppTypeId <= 0)
            {
                buildingPermit.PermitAppTypeId = await _dbContext.PermitApplicationTypes
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => x.Id)
                    .FirstAsync();
            }

            if (buildingPermit.BuildingPermitCategoryId <= 0)
            {
                buildingPermit.BuildingPermitCategoryId = await _dbContext.BuildingPermitCategories
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => x.Id)
                    .FirstAsync();
            }

            if (buildingPermit.OccupancyNatureId <= 0)
            {
                buildingPermit.OccupancyNatureId = await _dbContext.OccupancyNatures
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => x.Id)
                    .FirstAsync();
            }

            if (buildingPermit.ProjectClassId <= 0)
            {
                buildingPermit.ProjectClassId = await _dbContext.ProjectClassifications
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => x.Id)
                    .FirstAsync();
            }

            if (buildingPermit.ProvinceId <= 0)
            {
                buildingPermit.ProvinceId = await _dbContext.Provinces
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => x.Id)
                    .FirstAsync();
            }

            if (buildingPermit.LGUId <= 0)
            {
                buildingPermit.LGUId = await _dbContext.LGUs
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => x.Id)
                    .FirstAsync();
            }

            if (buildingPermit.BarangayId <= 0)
            {
                buildingPermit.BarangayId = await _dbContext.Barangays
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => x.Id)
                    .FirstAsync();
            }

            if (buildingPermit.AppInfo != null)
            {
                if (buildingPermit.AppInfo.ApplicantTypeId <= 0)
                {
                    buildingPermit.AppInfo.ApplicantTypeId = await _dbContext.ApplicantTypes
                        .AsNoTracking()
                        .OrderBy(x => x.Id)
                        .Select(x => x.Id)
                        .FirstAsync();
                }

                if (buildingPermit.AppInfo.OwnershipTypeId <= 0)
                {
                    buildingPermit.AppInfo.OwnershipTypeId = await _dbContext.OwnershipTypes
                        .AsNoTracking()
                        .OrderBy(x => x.Id)
                        .Select(x => x.Id)
                        .FirstAsync();
                }
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
