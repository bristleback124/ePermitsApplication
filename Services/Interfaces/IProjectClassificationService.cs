using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Services.Interfaces
{
    public interface IProjectClassificationService
    {
        Task<IEnumerable<ProjectClassification>> GetAllAsync();
        Task<ProjectClassification?> GetByIdAsync(int id);
        Task<ProjectClassification> CreateAsync(CreateProjectClassificationDto dto);
        Task<bool> UpdateAsync(int id, UpdateProjectClassificationDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<IEnumerable<ProjectClassification>> GetByNameAsync(
            string projectClassDesc,
            PaginationParams pagination);
        Task<PagedResult<ProjectClassification>> FilterByNameAsync(
            string projectClassDesc,
            PaginationParams pagination);
    }
}
