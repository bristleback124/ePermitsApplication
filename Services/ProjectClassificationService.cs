using AutoMapper;
using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Repositories;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Services
{
    public class ProjectClassificationService : IProjectClassificationService
    {
        private readonly IProjectClassificationRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly ApplicationDbContext _context;

        public ProjectClassificationService(
            IProjectClassificationRepository repository,
            IMapper mapper,
            ICurrentUserService currentUser,
            ApplicationDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
            _context = context;
        }

        public async Task<IEnumerable<ProjectClassification>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ProjectClassification?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<ProjectClassification> CreateAsync(CreateProjectClassificationDto dto)
        {
            var projectClassification = _mapper.Map<ProjectClassification>(dto);

            projectClassification.CreatedAt = DateTime.UtcNow;
            projectClassification.CreatedBy = _currentUser.UserName ?? "System";

            await _repository.AddAsync(projectClassification);
            await _repository.SaveChangesAsync();

            return projectClassification;
        }

        public async Task<bool> UpdateAsync(int id, UpdateProjectClassificationDto dto)
        {
            var projectClassification = await _repository.GetByIdAsync(id);
            if (projectClassification == null)
                return false;

            _mapper.Map(dto, projectClassification);

            projectClassification.UpdatedAt = DateTime.UtcNow;
            projectClassification.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(projectClassification);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var projectClassification = await _repository.GetByIdAsync(id);
            if (projectClassification == null)
                return false;

            var hasReferences = await _context.BuildingPermits.AnyAsync(x => x.ProjectClassId == id);
            if (hasReferences)
                throw new InvalidOperationException("This project classification is already referenced by existing applications. Deactivate it instead of deleting it.");

            projectClassification.IsDeleted = true;
            projectClassification.UpdatedAt = DateTime.UtcNow;
            projectClassification.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(projectClassification);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var projectClassification = await _repository.GetByIdIncludingDeletedAsync(id);
            if (projectClassification == null || !projectClassification.IsDeleted)
                return false;

            projectClassification.IsDeleted = false;
            projectClassification.UpdatedAt = DateTime.UtcNow;
            projectClassification.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(projectClassification);
            return await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProjectClassification>> GetByNameAsync(
            string projectClassDesc,
            PaginationParams pagination)
        {
            return await _repository.GetByNameAsync(projectClassDesc, pagination);
        }

        public async Task<PagedResult<ProjectClassification>> FilterByNameAsync(
            string projectClassDesc,
            PaginationParams pagination)
        {
            return await _repository.FilterByNameAsync(projectClassDesc, pagination);
        }
    }
}
