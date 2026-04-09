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
using Microsoft.Extensions.Options;
using ePermits.Data;

namespace ePermitsApp.Services
{
    public class BuildingPermitService : IBuildingPermitService
    {
        private readonly IBuildingPermitRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepository _userRepository;
        private readonly FileStorageSettings _fileSettings;
        private readonly IEmailService _emailService;
        private readonly IAdminEmailNotificationConfigService _adminEmailNotificationConfigService;
        private readonly ILogger<BuildingPermitService> _logger;

        public BuildingPermitService(
            IBuildingPermitRepository repository,
            IMapper mapper,
            ICurrentUserService currentUser,
            IUserRepository userRepository,
            IOptions<FileStorageSettings> fileSettings,
            IEmailService emailService,
            IAdminEmailNotificationConfigService adminEmailNotificationConfigService,
            ILogger<BuildingPermitService> logger)
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

        public async Task<BuildingPermit> CreateAsync(BuildingPermitCreateDto dto)
        {
            ValidateCategorySpecificRequirements(dto);
            var buildingPermit = _mapper.Map<BuildingPermit>(dto);

            var now = DateTime.UtcNow;
            int currentUserId = 15;
            if (int.TryParse(_currentUser.UserId, out int id))
            {
                currentUserId = id;
            }

            buildingPermit.CreatedAt = now;
            buildingPermit.CreatedBy = currentUserId;

            buildingPermit.Application = new Application
            {
                UserId = currentUserId,
                Type = ApplicationWorkflowDefinitions.PermitTypes.BuildingPermit,
                Status = ApplicationWorkflowDefinitions.OverallStatuses.Submitted,
                CreatedAt = now,
                CreatedBy = _currentUser.UserName ?? "System",
                DepartmentReviews = ApplicationWorkflowDefinitions
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

            // Set formatted ID now that we have the auto-generated Id
            buildingPermit.Application.FormattedId = $"BP-{buildingPermit.Application.CreatedAt.Year}-{buildingPermit.Application.Id:D3}";

            // Save files
            if (buildingPermit.AppInfo != null)
            {
                buildingPermit.AppInfo.ReqDocProofOwnership = await SaveFileWithMetadataAsync(dto.AppInfo.ReqDocProofOwnership, buildingPermit.Id, "req-docs");
                buildingPermit.AppInfo.ReqDocBarangayClearance = await SaveFileWithMetadataAsync(dto.AppInfo.ReqDocBarangayClearance, buildingPermit.Id, "req-docs");
                buildingPermit.AppInfo.ReqDocTaxDeclaration = await SaveFileWithMetadataAsync(dto.AppInfo.ReqDocTaxDeclaration, buildingPermit.Id, "req-docs");
                buildingPermit.AppInfo.ReqDocRealPropTaxReceipt = await SaveFileWithMetadataAsync(dto.AppInfo.ReqDocRealPropTaxReceipt, buildingPermit.Id, "req-docs");

                if (dto.AppInfo.ReqDocECCorCNC != null)
                    buildingPermit.AppInfo.ReqDocECCorCNC = await SaveFileWithMetadataAsync(dto.AppInfo.ReqDocECCorCNC, buildingPermit.Id, "req-docs");

                if (dto.AppInfo.ReqDocSpecialClearances != null)
                    buildingPermit.AppInfo.ReqDocSpecialClearances = await SaveFileWithMetadataAsync(dto.AppInfo.ReqDocSpecialClearances, buildingPermit.Id, "req-docs");
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

            ValidateCategorySpecificRequirements(buildingPermit);

            // Update again with file paths
            _repository.Update(buildingPermit);
            if (buildingPermit.AppInfo != null) _repository.Update(buildingPermit); // This might be redundant but ensuring graph is marked
            await _repository.SaveChangesAsync();

            // Send notification emails
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

            return buildingPermit;
        }

        public async Task<(bool Success, string Message, BuildingPermit? BuildingPermit)> UpdateByApplicationIdAsync(int applicationId, BuildingPermitUpdateDto dto)
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

            await UpdateSingleFileAsync(buildingPermit.AppInfo, nameof(buildingPermit.AppInfo.ReqDocProofOwnership), dto.AppInfo.ReqDocProofOwnership, dto.AppInfo.KeepReqDocProofOwnership, buildingPermit.Id, true);
            await UpdateSingleFileAsync(buildingPermit.AppInfo, nameof(buildingPermit.AppInfo.ReqDocBarangayClearance), dto.AppInfo.ReqDocBarangayClearance, dto.AppInfo.KeepReqDocBarangayClearance, buildingPermit.Id, true);
            await UpdateSingleFileAsync(buildingPermit.AppInfo, nameof(buildingPermit.AppInfo.ReqDocTaxDeclaration), dto.AppInfo.ReqDocTaxDeclaration, dto.AppInfo.KeepReqDocTaxDeclaration, buildingPermit.Id, true);
            await UpdateSingleFileAsync(buildingPermit.AppInfo, nameof(buildingPermit.AppInfo.ReqDocRealPropTaxReceipt), dto.AppInfo.ReqDocRealPropTaxReceipt, dto.AppInfo.KeepReqDocRealPropTaxReceipt, buildingPermit.Id, true);
            await UpdateSingleFileAsync(buildingPermit.AppInfo, nameof(buildingPermit.AppInfo.ReqDocECCorCNC), dto.AppInfo.ReqDocECCorCNC, dto.AppInfo.KeepReqDocECCorCNC, buildingPermit.Id, false);
            await UpdateSingleFileAsync(buildingPermit.AppInfo, nameof(buildingPermit.AppInfo.ReqDocSpecialClearances), dto.AppInfo.ReqDocSpecialClearances, dto.AppInfo.KeepReqDocSpecialClearances, buildingPermit.Id, false);

            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocIoCPlans), dto.TechDoc.KeepTechDocIoCPlans, dto.TechDoc.TechDocIoCPlans, buildingPermit.Id, true);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocSEPlans), dto.TechDoc.KeepTechDocSEPlans, dto.TechDoc.TechDocSEPlans, buildingPermit.Id, true);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocEEPlans), dto.TechDoc.KeepTechDocEEPlans, dto.TechDoc.TechDocEEPlans, buildingPermit.Id, true);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocSPPlans), dto.TechDoc.KeepTechDocSPPlans, dto.TechDoc.TechDocSPPlans, buildingPermit.Id, true);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocStructuralAnalysisDesign), dto.TechDoc.KeepTechDocStructuralAnalysisDesign, dto.TechDoc.TechDocStructuralAnalysisDesign, buildingPermit.Id, false);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocFireSafetyPlans), dto.TechDoc.KeepTechDocFireSafetyPlans, dto.TechDoc.TechDocFireSafetyPlans, buildingPermit.Id, false);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocEnvironmentalDocuments), dto.TechDoc.KeepTechDocEnvironmentalDocuments, dto.TechDoc.TechDocEnvironmentalDocuments, buildingPermit.Id, false);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocSoilTestFieldDensityTest), dto.TechDoc.KeepTechDocSoilTestFieldDensityTest, dto.TechDoc.TechDocSoilTestFieldDensityTest, buildingPermit.Id, false);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocBOMCost), dto.TechDoc.KeepTechDocBOMCost, dto.TechDoc.TechDocBOMCost, buildingPermit.Id, true);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocSoW), dto.TechDoc.KeepTechDocSoW, dto.TechDoc.TechDocSoW, buildingPermit.Id, true);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocMEPlans), dto.TechDoc.KeepTechDocMEPlans, dto.TechDoc.TechDocMEPlans, buildingPermit.Id, false);
            await UpdateMultiFileAsync(buildingPermit.TechDoc, nameof(buildingPermit.TechDoc.TechDocECEPlans), dto.TechDoc.KeepTechDocECEPlans, dto.TechDoc.TechDocECEPlans, buildingPermit.Id, false);

            await UpdateSingleFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocZoningClearance), dto.SupportingDoc.SupportDocZoningClearance, dto.SupportingDoc.KeepSupportDocZoningClearance, buildingPermit.Id, false, "supporting-docs");
            await UpdateSingleFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocLocationalClearance), dto.SupportingDoc.SupportDocLocationalClearance, dto.SupportingDoc.KeepSupportDocLocationalClearance, buildingPermit.Id, false, "supporting-docs");
            await UpdateSingleFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocFireSafetyClearance), dto.SupportingDoc.SupportDocFireSafetyClearance, dto.SupportingDoc.KeepSupportDocFireSafetyClearance, buildingPermit.Id, false, "supporting-docs");
            await UpdateSingleFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocHighwayClearance), dto.SupportingDoc.SupportDocHighwayClearance, dto.SupportingDoc.KeepSupportDocHighwayClearance, buildingPermit.Id, false, "supporting-docs");
            await UpdateSingleFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocHeightClearance), dto.SupportingDoc.SupportDocHeightClearance, dto.SupportingDoc.KeepSupportDocHeightClearance, buildingPermit.Id, false, "supporting-docs");
            await UpdateSingleFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocECCorCNC), dto.SupportingDoc.SupportDocECCorCNC, dto.SupportingDoc.KeepSupportDocECCorCNC, buildingPermit.Id, false, "supporting-docs");
            await UpdateSingleFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocDENRClearance), dto.SupportingDoc.SupportDocDENRClearance, dto.SupportingDoc.KeepSupportDocDENRClearance, buildingPermit.Id, false, "supporting-docs");
            await UpdateSingleFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocSECRegistration), dto.SupportingDoc.SupportDocSECRegistration, dto.SupportingDoc.KeepSupportDocSECRegistration, buildingPermit.Id, false, "supporting-docs");
            await UpdateSingleFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocBoardResolution), dto.SupportingDoc.SupportDocBoardResolution, dto.SupportingDoc.KeepSupportDocBoardResolution, buildingPermit.Id, false, "supporting-docs");
            await UpdateSingleFileAsync(buildingPermit.SupportingDoc, nameof(buildingPermit.SupportingDoc.SupportDocHOAClearance), dto.SupportingDoc.SupportDocHOAClearance, dto.SupportingDoc.KeepSupportDocHOAClearance, buildingPermit.Id, false, "supporting-docs");

            ValidateCategorySpecificRequirements(buildingPermit);

            buildingPermit.Application.UpdatedAt = now;
            buildingPermit.Application.UpdatedBy = actor;

            _repository.Update(buildingPermit);
            await _repository.SaveChangesAsync();

            return (true, "Application updated successfully", buildingPermit);
        }

        private async Task<string> SaveFilesAsync(IFormFileCollection? files, int permitId, string subFolder)
        {
            if (files == null || files.Count == 0)
                return string.Empty;

            var metadataList = new List<FileMetadataDto>();

            foreach (var file in files)
            {
                var filePath = await SaveFileInternalAsync(file, permitId, subFolder);
                if (!string.IsNullOrEmpty(filePath))
                {
                    metadataList.Add(new FileMetadataDto
                    {
                        Name = file.FileName,
                        Size = file.Length,
                        Path = filePath
                    });
                }
            }

            return FilePathHelper.Serialize(metadataList);
        }

        private async Task<string> SaveFileWithMetadataAsync(IFormFile file, int permitId, string subFolder)
        {
            var filePath = await SaveFileInternalAsync(file, permitId, subFolder);
            if (string.IsNullOrEmpty(filePath)) return string.Empty;

            return FilePathHelper.SerializeSingle(new FileMetadataDto
            {
                Name = file.FileName,
                Size = file.Length,
                Path = filePath
            });
        }

        private async Task<string> SaveFileInternalAsync(IFormFile file, int permitId, string subFolder)
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
            buildingPermit.DateofSignature = dto.DateofSignature;
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

        private async Task UpdateSingleFileAsync(object target, string propertyName, IFormFile? newFile, bool keepExisting, int permitId, bool required, string subFolder = "req-docs")
        {
            var property = target.GetType().GetProperty(propertyName)!;
            var currentValue = property.GetValue(target) as string;

            if (newFile != null)
            {
                property.SetValue(target, await SaveFileWithMetadataAsync(newFile, permitId, subFolder));
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

        private async Task UpdateSupportingDocFilesAsync(BuildingPermitSupportingDoc supportingDoc, BuildingPermitSupportingDocCreateDto dto, int permitId)
        {
            supportingDoc.SupportDocZoningClearance = await SaveOptionalFileAsync(dto.SupportDocZoningClearance, permitId);
            supportingDoc.SupportDocLocationalClearance = await SaveOptionalFileAsync(dto.SupportDocLocationalClearance, permitId);
            supportingDoc.SupportDocFireSafetyClearance = await SaveOptionalFileAsync(dto.SupportDocFireSafetyClearance, permitId);
            supportingDoc.SupportDocHighwayClearance = await SaveOptionalFileAsync(dto.SupportDocHighwayClearance, permitId);
            supportingDoc.SupportDocHeightClearance = await SaveOptionalFileAsync(dto.SupportDocHeightClearance, permitId);
            supportingDoc.SupportDocECCorCNC = await SaveOptionalFileAsync(dto.SupportDocECCorCNC, permitId);
            supportingDoc.SupportDocDENRClearance = await SaveOptionalFileAsync(dto.SupportDocDENRClearance, permitId);
            supportingDoc.SupportDocSECRegistration = await SaveOptionalFileAsync(dto.SupportDocSECRegistration, permitId);
            supportingDoc.SupportDocBoardResolution = await SaveOptionalFileAsync(dto.SupportDocBoardResolution, permitId);
            supportingDoc.SupportDocHOAClearance = await SaveOptionalFileAsync(dto.SupportDocHOAClearance, permitId);
        }

        private async Task<string> SaveOptionalFileAsync(IFormFile? file, int permitId)
        {
            if (file == null)
            {
                return string.Empty;
            }

            return await SaveFileWithMetadataAsync(file, permitId, "supporting-docs");
        }

        private async Task UpdateMultiFileAsync(object target, string propertyName, string[] keepPaths, IFormFileCollection? newFiles, int permitId, bool required)
        {
            var property = target.GetType().GetProperty(propertyName)!;
            var existing = FilePathHelper.Deserialize(property.GetValue(target) as string);
            var retained = existing.Where(file => keepPaths.Contains(file.Path, StringComparer.OrdinalIgnoreCase)).ToList();

            if (newFiles != null && newFiles.Count > 0)
            {
                retained.AddRange(await SaveNewFileMetadataAsync(newFiles, permitId, "tech-docs"));
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
                var path = await SaveFileInternalAsync(file, permitId, subFolder);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    result.Add(new FileMetadataDto
                    {
                        Name = file.FileName,
                        Size = file.Length,
                        Path = path
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

        private static void ValidateCategorySpecificRequirements(BuildingPermitCreateDto dto)
        {
            var categoryId = dto.BuildingPermitCategoryId;
            if (dto.Accessories == null || dto.Accessories.Length == 0)
            {
                throw new InvalidOperationException("Accessories is required.");
            }

            var isComplex = categoryId == 2;
            var isHighlyTechnical = categoryId == 3;

            if (isHighlyTechnical && string.IsNullOrWhiteSpace(dto.DesignProf.MEFullName))
            {
                throw new InvalidOperationException("Mechanical Engineer is required.");
            }

            if ((isComplex || isHighlyTechnical) && string.IsNullOrWhiteSpace(dto.DesignProf.GSEFullName))
            {
                throw new InvalidOperationException("Geotechnical / Soil Engineer is required.");
            }

            if (isComplex || isHighlyTechnical)
            {
                RequireFiles(dto.TechDoc.TechDocStructuralAnalysisDesign, "Structural Analysis & Design");
                RequireFiles(dto.TechDoc.TechDocFireSafetyPlans, "Fire Safety Plans");
                RequireFiles(dto.TechDoc.TechDocEnvironmentalDocuments, "Environmental Documents");
            }

            if (isHighlyTechnical)
            {
                RequireFiles(dto.TechDoc.TechDocSoilTestFieldDensityTest, "Soil Test / Field Density Test");
            }

            var supportingDoc = dto.SupportingDoc;
            var requiredSupportingDocs = isComplex || isHighlyTechnical
                ? ComplexSupportingDocFields
                : SimpleSupportingDocFields;

            foreach (var field in requiredSupportingDocs)
            {
                var file = supportingDoc.GetType().GetProperty(field)?.GetValue(supportingDoc) as IFormFile;
                if (file == null)
                {
                    throw new InvalidOperationException($"{field} is required.");
                }
            }

            if (dto.AppInfo.ApplicantTypeId == 2 && dto.SupportingDoc.SupportDocSECRegistration == null)
            {
                throw new InvalidOperationException("SupportDocSECRegistration is required.");
            }
        }

        private static void RequireFiles(IFormFileCollection? files, string label)
        {
            if (files == null || files.Count == 0)
            {
                throw new InvalidOperationException($"{label} is required.");
            }
        }

        private static void ValidateCategorySpecificRequirements(BuildingPermit buildingPermit)
        {
            var categoryId = buildingPermit.BuildingPermitCategoryId;
            if (string.IsNullOrWhiteSpace(buildingPermit.Accessories))
            {
                throw new InvalidOperationException("Accessories is required.");
            }

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

            if ((isComplex || isHighlyTechnical) && string.IsNullOrWhiteSpace(designProf.GSEFullName))
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
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidOperationException($"{field} is required.");
                }
            }

            if (buildingPermit.AppInfo?.ApplicantTypeId == 2 && string.IsNullOrWhiteSpace(supportingDoc.SupportDocSECRegistration))
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

            if (!string.Equals(application.Status, ApplicationWorkflowDefinitions.OverallStatuses.Submitted, StringComparison.OrdinalIgnoreCase))
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
