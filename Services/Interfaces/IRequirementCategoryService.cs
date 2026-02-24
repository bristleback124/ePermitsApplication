using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Services.Interfaces
{
    public interface IRequirementCategoryService
    {
        Task<IEnumerable<RequirementCategory>> GetAllAsync();
        Task<RequirementCategory?> GetByIdAsync(int id);
        Task<RequirementCategory> CreateAsync(CreateRequirementCategoryDto dto);
        Task<bool> UpdateAsync(int id, UpdateRequirementCategoryDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<IEnumerable<RequirementCategory>> GetByReqClassAsync(int reqClassId);
        Task<IEnumerable<RequirementCategory>> GetByReqClassDescAsync(
            string reqClassDesc,
            PaginationParams pagination);
        Task<PagedResult<RequirementCategory>> FilterByReqClassDescAsync(
            string reqClassDesc,
            PaginationParams pagination);
    }
}
