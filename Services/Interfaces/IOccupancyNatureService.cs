using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Services.Interfaces
{
    public interface IOccupancyNatureService
    {
        Task<IEnumerable<OccupancyNature>> GetAllAsync();
        Task<OccupancyNature?> GetByIdAsync(int id);
        Task<OccupancyNature> CreateAsync(CreateOccupancyNatureDto dto);
        Task<bool> UpdateAsync(int id, UpdateOccupancyNatureDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<IEnumerable<OccupancyNature>> GetByNameAsync(
            string occupancyNatureDesc,
            PaginationParams pagination);
        Task<PagedResult<OccupancyNature>> FilterByNameAsync(
            string occupancyNatureDesc,
            PaginationParams pagination);
    }
}
