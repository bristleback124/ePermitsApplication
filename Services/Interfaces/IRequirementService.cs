using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Services.Interfaces
{
    public interface IRequirementService
    {
        Task<IEnumerable<Requirement>> GetAllAsync();
        Task<Requirement?> GetByIdAsync(int id);
        Task<Requirement> CreateAsync(CreateRequirementDto dto);
        Task<bool> UpdateAsync(int id, UpdateRequirementDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<PagedResult<Requirement>> FilterAsync(
            string? reqDesc,
            int? reqCatId,
            string? reqClassDesc,
            PaginationParams pagination);
    }
}
