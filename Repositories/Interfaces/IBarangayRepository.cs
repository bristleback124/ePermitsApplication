using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IBarangayRepository
    {
        Task<IEnumerable<Barangay>> GetAllAsync();
        Task<Barangay?> GetByIdAsync(int id);
        Task AddAsync(Barangay barangay);
        void Update(Barangay barangay);
        Task<bool> SaveChangesAsync();
        Task<Barangay?> GetByIdIncludingDeletedAsync(int id);
        Task<PagedResult<Barangay>> FilterAsync(
            string? barangayName,
            int? lguId,
            string? provinceName,
            PaginationParams pagination);
        Task<IEnumerable<Barangay>> GetDeletedByLGUAsync(int lguId);
        Task<IEnumerable<Barangay>> GetByLGUAsync(int lguId);
    }
}
