using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IRequirementRepository
    {
        Task<IEnumerable<Requirement>> GetAllAsync();
        Task<Requirement?> GetByIdAsync(int id);
        Task AddAsync(Requirement requirement);
        void Update(Requirement requirement);
        Task<bool> SaveChangesAsync();
        Task<Requirement?> GetByIdIncludingDeletedAsync(int id);
        Task<PagedResult<Requirement>> FilterAsync(
            string? reqDesc,
            int? reqCatId,
            string? reqClassDesc,
            PaginationParams pagination);
        Task<IEnumerable<Requirement>> GetDeletedByReqCatAsync(int lguId);
        Task<IEnumerable<Requirement>> GetByReqCatAsync(int lguId);
    }
}
