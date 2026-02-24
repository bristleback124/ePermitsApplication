using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Services.Interfaces
{
    public interface IBarangayService
    {
        Task<IEnumerable<Barangay>> GetAllAsync();
        Task<Barangay?> GetByIdAsync(int id);
        Task<Barangay> CreateAsync(CreateBarangayDto dto);
        Task<bool> UpdateAsync(int id, UpdateBarangayDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<PagedResult<Barangay>> FilterAsync(
            string? barangayName,
            int? lguId,
            string? provinceName,
            PaginationParams pagination);
    }
}
