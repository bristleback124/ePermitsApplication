using ePermitsApp.Entities.CoOApp;
using ePermitsApp.DTOs;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface ICoOAppRepository
    {
        Task<PagedResult<CoOApp>> GetAllAsync(PaginationParams pagination);
        Task<CoOApp?> GetByIdAsync(int id);
        Task AddAsync(CoOApp coOApp);
        void Update(CoOApp coOApp);
        Task<bool> SaveChangesAsync();
    }
}
