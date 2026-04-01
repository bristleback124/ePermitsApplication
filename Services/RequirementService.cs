using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Helpers;
using ePermitsApp.Repositories;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class RequirementService : IRequirementService
    {
        private readonly IRequirementRepository _repository;
        private readonly IRequirementCategoryRepository _reqCatRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public RequirementService(
            IRequirementRepository repository,
            IRequirementCategoryRepository reqCatRepository,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _reqCatRepository = reqCatRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<Requirement>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Requirement?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Requirement> CreateAsync(CreateRequirementDto dto)
        {
            var reqCat = await _reqCatRepository.GetByIdAsync(dto.ReqCatId);
            if (reqCat == null)
                throw new Exception("Requirement Category not found");

            var requirement = _mapper.Map<Requirement>(dto);
            requirement.ApplicationTypeScope = NormalizeScope(dto.ApplicationTypeScope);
            requirement.CreatedAt = DateTime.UtcNow;
            requirement.CreatedBy = _currentUser.UserName ?? "System";

            await _repository.AddAsync(requirement);
            await _repository.SaveChangesAsync();

            return requirement;
        }

        public async Task<bool> UpdateAsync(int id, UpdateRequirementDto dto)
        {
            var requirement = await _repository.GetByIdAsync(id);
            if (requirement == null)
                return false;

            var reqCatExists = await _reqCatRepository.GetByIdAsync(dto.ReqCatId);
            if (reqCatExists == null)
                throw new Exception("Requirement Category not found");

            _mapper.Map(dto, requirement);
            requirement.ApplicationTypeScope = NormalizeScope(dto.ApplicationTypeScope);
            requirement.UpdatedAt = DateTime.UtcNow;
            requirement.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(requirement);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var requirement = await _repository.GetByIdAsync(id);
            if (requirement == null)
                return false;

            requirement.IsDeleted = true;
            requirement.UpdatedAt = DateTime.UtcNow;
            requirement.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(requirement);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var requirement = await _repository.GetByIdIncludingDeletedAsync(id);
            if (requirement == null || !requirement.IsDeleted)
                return false;

            if (requirement.RequirementCategory.IsDeleted || requirement.RequirementCategory.RequirementClassification.IsDeleted)
                return false;

            requirement.IsDeleted = false;
            requirement.UpdatedAt = DateTime.UtcNow;
            requirement.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(requirement);
            return await _repository.SaveChangesAsync();
        }

        public async Task<PagedResult<Requirement>> FilterAsync(
            string? reqDesc,
            int? reqCatId,
            string? reqClassDesc,
            PaginationParams pagination)
        {
            return await _repository.FilterAsync(reqDesc, reqCatId, reqClassDesc, pagination);
        }

        private static string NormalizeScope(string? scope)
        {
            if (!string.IsNullOrWhiteSpace(scope) && !MaintenanceApplicationScopes.IsValid(scope))
                throw new InvalidOperationException("ApplicationTypeScope must be BuildingPermit, CertificateOfOccupancy, or Both.");

            return MaintenanceApplicationScopes.Normalize(scope);
        }
    }
}
