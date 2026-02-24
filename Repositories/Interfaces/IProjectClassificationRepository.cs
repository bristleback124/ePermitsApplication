using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IProjectClassificationRepository
    {
        Task<IEnumerable<ProjectClassification>> GetAllAsync();
        Task<ProjectClassification?> GetByIdAsync(int id);
        Task AddAsync(ProjectClassification projectClassification);
        void Update(ProjectClassification projectClassification);
        Task<bool> SaveChangesAsync();
        Task<ProjectClassification?> GetByIdIncludingDeletedAsync(int id);
        Task<IEnumerable<ProjectClassification>> GetByNameAsync(
            string projectClassDesc,
            PaginationParams pagination);
        Task<PagedResult<ProjectClassification>> FilterByNameAsync(
            string projectClassDesc,
            PaginationParams pagination);
    }
}
