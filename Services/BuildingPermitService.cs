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

namespace ePermitsApp.Services
{
    public class BuildingPermitService : IBuildingPermitService
    {
        private readonly IBuildingPermitRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly FileStorageSettings _fileSettings;
        private readonly IEmailService _emailService;
        private readonly ILogger<BuildingPermitService> _logger;

        public BuildingPermitService(
            IBuildingPermitRepository repository,
            IMapper mapper,
            ICurrentUserService currentUser,
            IOptions<FileStorageSettings> fileSettings,
            IEmailService emailService,
            ILogger<BuildingPermitService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
            _fileSettings = fileSettings.Value;
            _emailService = emailService;
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

        public async Task<BuildingPermit> CreateAsync(BuildingPermitCreateDto dto)
        {
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
                buildingPermit.TechDoc.TechDocBOMCost = await SaveFilesAsync(dto.TechDoc.TechDocBOMCost, buildingPermit.Id, "tech-docs");
                buildingPermit.TechDoc.TechDocSoW = await SaveFilesAsync(dto.TechDoc.TechDocSoW, buildingPermit.Id, "tech-docs");

                if (dto.TechDoc.TechDocMEPlans != null)
                    buildingPermit.TechDoc.TechDocMEPlans = await SaveFilesAsync(dto.TechDoc.TechDocMEPlans, buildingPermit.Id, "tech-docs");

                if (dto.TechDoc.TechDocECEPlans != null)
                    buildingPermit.TechDoc.TechDocECEPlans = await SaveFilesAsync(dto.TechDoc.TechDocECEPlans, buildingPermit.Id, "tech-docs");
            }

            // Update again with file paths
            _repository.Update(buildingPermit);
            if (buildingPermit.AppInfo != null) _repository.Update(buildingPermit); // This might be redundant but ensuring graph is marked
            await _repository.SaveChangesAsync();

            // Send submission confirmation email
            if (buildingPermit.AppInfo != null && !string.IsNullOrEmpty(buildingPermit.AppInfo.Email))
            {
                try
                {
                    await _emailService.SendTemplatedEmailAsync(
                        buildingPermit.AppInfo.Email,
                        "Your Building Permit Application Has Been Submitted",
                        "ApplicationSubmitted",
                        new ApplicationSubmittedModel
                        {
                            ApplicantName = buildingPermit.AppInfo.FullName,
                            ApplicationType = "Building Permit",
                            FormattedId = buildingPermit.Application.FormattedId,
                            SubmittedAt = now
                        });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send submission email for Building Permit {FormattedId}", buildingPermit.Application.FormattedId);
                }
            }

            return buildingPermit;
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
    }
}
