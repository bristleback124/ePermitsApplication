using ePermitsApp.DTOs;
using ePermitsApp.Entities.CoOApp;

namespace ePermitsApp.Services.Interfaces
{
    public interface ICoOAppService
    {
        Task<PagedResult<CoOApp>> GetAllAsync(PaginationParams pagination);
        Task<CoOApp?> GetByIdAsync(int id);
        Task<CoOApp> CreateAsync(CoOAppCreateDto dto);
    }
}
