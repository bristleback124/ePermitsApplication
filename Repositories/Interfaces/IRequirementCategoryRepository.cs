using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IRequirementCategoryRepository
    {
        Task<IEnumerable<RequirementCategory>> GetAllAsync();
        Task<RequirementCategory?> GetByIdAsync(int id);
        Task AddAsync(RequirementCategory requirementCategory);
        void Update(RequirementCategory requirementCategory);
        Task<bool> SaveChangesAsync();
        Task<RequirementCategory?> GetByIdIncludingDeletedAsync(int id);
        Task<IEnumerable<RequirementCategory>> GetByReqClassAsync(int reqClassId);
        Task<IEnumerable<RequirementCategory>> GetByReqClassIncludingDeletedAsync(int reqClassId);
        Task<IEnumerable<RequirementCategory>> GetByReqClassDescAsync(
            string reqClassDesc,
            PaginationParams pagination);
        Task<PagedResult<RequirementCategory>> FilterByReqClassDescAsync(
            string reqClassDesc,
            PaginationParams pagination);
    }
}
