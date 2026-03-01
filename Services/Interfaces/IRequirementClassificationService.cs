using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Services.Interfaces
{
    public interface IRequirementClassificationService
    {
        Task<IEnumerable<RequirementClassification>> GetAllAsync();
        Task<RequirementClassification?> GetByIdAsync(int id);
        Task<RequirementClassification> CreateAsync(CreateRequirementClassificationDto dto);
        Task<bool> UpdateAsync(int id, UpdateRequirementClassificationDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<IEnumerable<RequirementClassification>> GetByNameAsync(
            string reqClassDesc,
            PaginationParams pagination);
        Task<PagedResult<RequirementClassification>> FilterByNameAsync(
            string reqClassDesc,
            PaginationParams pagination);
        Task<IEnumerable<RequirementClassification>> GetAllWithHierarchyAsync();
    }
}
