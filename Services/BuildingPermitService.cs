using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities.BuildingPermit;
using ePermits.Models;
using ePermitsApp.Helpers;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ePermitsApp.Services
{
    public class BuildingPermitService : IBuildingPermitService
    {
        private readonly IBuildingPermitRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly FileStorageSettings _fileSettings;

        public BuildingPermitService(
            IBuildingPermitRepository repository,
            IMapper mapper,
            ICurrentUserService currentUser,
            IOptions<FileStorageSettings> fileSettings)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
            _fileSettings = fileSettings.Value;
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
                Type = "BuildingPermit",
                Status = "Pending",
                CreatedAt = now,
                CreatedBy = _currentUser.UserName ?? "System"
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

            // Save files
            if (buildingPermit.AppInfo != null)
            {
                buildingPermit.AppInfo.ReqDocProofOwnership = await SaveFileAsync(dto.AppInfo.ReqDocProofOwnership, buildingPermit.Id, "req-docs");
                buildingPermit.AppInfo.ReqDocBarangayClearance = await SaveFileAsync(dto.AppInfo.ReqDocBarangayClearance, buildingPermit.Id, "req-docs");
                buildingPermit.AppInfo.ReqDocTaxDeclaration = await SaveFileAsync(dto.AppInfo.ReqDocTaxDeclaration, buildingPermit.Id, "req-docs");
                buildingPermit.AppInfo.ReqDocRealPropTaxReceipt = await SaveFileAsync(dto.AppInfo.ReqDocRealPropTaxReceipt, buildingPermit.Id, "req-docs");

                if (dto.AppInfo.ReqDocECCorCNC != null)
                    buildingPermit.AppInfo.ReqDocECCorCNC = await SaveFileAsync(dto.AppInfo.ReqDocECCorCNC, buildingPermit.Id, "req-docs");

                if (dto.AppInfo.ReqDocSpecialClearances != null)
                    buildingPermit.AppInfo.ReqDocSpecialClearances = await SaveFileAsync(dto.AppInfo.ReqDocSpecialClearances, buildingPermit.Id, "req-docs");
            }

            if (buildingPermit.TechDoc != null)
            {
                buildingPermit.TechDoc.TechDocIoCPlans = await SaveFileAsync(dto.TechDoc.TechDocIoCPlans, buildingPermit.Id, "tech-docs");
                buildingPermit.TechDoc.TechDocSEPlans = await SaveFileAsync(dto.TechDoc.TechDocSEPlans, buildingPermit.Id, "tech-docs");
                buildingPermit.TechDoc.TechDocEEPlans = await SaveFileAsync(dto.TechDoc.TechDocEEPlans, buildingPermit.Id, "tech-docs");
                buildingPermit.TechDoc.TechDocSPPlans = await SaveFileAsync(dto.TechDoc.TechDocSPPlans, buildingPermit.Id, "tech-docs");
                buildingPermit.TechDoc.TechDocBOMCost = await SaveFileAsync(dto.TechDoc.TechDocBOMCost, buildingPermit.Id, "tech-docs");
                buildingPermit.TechDoc.TechDocSoW = await SaveFileAsync(dto.TechDoc.TechDocSoW, buildingPermit.Id, "tech-docs");

                if (dto.TechDoc.TechDocMEPlans != null)
                    buildingPermit.TechDoc.TechDocMEPlans = await SaveFileAsync(dto.TechDoc.TechDocMEPlans, buildingPermit.Id, "tech-docs");

                if (dto.TechDoc.TechDocECEPlans != null)
                    buildingPermit.TechDoc.TechDocECEPlans = await SaveFileAsync(dto.TechDoc.TechDocECEPlans, buildingPermit.Id, "tech-docs");
            }

            // Update again with file paths
            _repository.Update(buildingPermit);
            await _repository.SaveChangesAsync();

            return buildingPermit;
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
