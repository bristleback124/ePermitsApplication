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

namespace ePermitsApp.Services
{
    public class CoOAppService : ICoOAppService
    {
        private readonly ICoOAppRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly FileStorageSettings _fileSettings;
        private readonly IEmailService _emailService;
        private readonly ILogger<CoOAppService> _logger;

        public CoOAppService(
            ICoOAppRepository repository,
            IMapper mapper,
            ICurrentUserService currentUser,
            IOptions<FileStorageSettings> fileSettings,
            IEmailService emailService,
            ILogger<CoOAppService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
            _fileSettings = fileSettings.Value;
            _emailService = emailService;
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

            // Send submission confirmation email
            if (!string.IsNullOrEmpty(coOApp.Email))
            {
                try
                {
                    await _emailService.SendTemplatedEmailAsync(
                        coOApp.Email,
                        "Your Certificate of Occupancy Application Has Been Submitted",
                        "ApplicationSubmitted",
                        new ApplicationSubmittedModel
                        {
                            ApplicantName = coOApp.FullName,
                            ApplicationType = "Certificate of Occupancy",
                            FormattedId = coOApp.Application.FormattedId,
                            SubmittedAt = now
                        });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send submission email for CoO {FormattedId}", coOApp.Application.FormattedId);
                }
            }

            return coOApp;
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
    }
}
