using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IOccupancyNatureRepository
    {
        Task<IEnumerable<OccupancyNature>> GetAllAsync();
        Task<OccupancyNature?> GetByIdAsync(int id);
        Task AddAsync(OccupancyNature occupancyNature);
        void Update(OccupancyNature occupancyNature);
        Task<bool> SaveChangesAsync();
        Task<OccupancyNature?> GetByIdIncludingDeletedAsync(int id);
        Task<IEnumerable<OccupancyNature>> GetByNameAsync(
            string occupancyNatureDesc,
            PaginationParams pagination);
        Task<PagedResult<OccupancyNature>> FilterByNameAsync(
            string occupancyNatureDesc,
            PaginationParams pagination);
    }
}
