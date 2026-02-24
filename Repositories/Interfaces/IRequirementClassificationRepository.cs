using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IRequirementClassificationRepository
    {
        Task<IEnumerable<RequirementClassification>> GetAllAsync();
        Task<RequirementClassification?> GetByIdAsync(int id);
        Task AddAsync(RequirementClassification reqClass);
        void Update(RequirementClassification reqClass);
        Task<bool> SaveChangesAsync();
        Task<RequirementClassification?> GetByIdIncludingDeletedAsync(int id);
        Task<IEnumerable<RequirementClassification>> GetByNameAsync(
            string reqClassDesc,
            PaginationParams pagination);
        Task<PagedResult<RequirementClassification>> FilterByNameAsync(
            string reqClassDesc,
            PaginationParams pagination);
    }
}
