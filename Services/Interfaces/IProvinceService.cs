using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Services.Interfaces
{
    public interface IProvinceService
    {
        Task<IEnumerable<Province>> GetAllAsync();
        Task<Province?> GetByIdAsync(int id);
        Task<Province> CreateAsync(CreateProvinceDto dto);
        Task<bool> UpdateAsync(int id, UpdateProvinceDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<IEnumerable<Province>> GetByNameAsync(
            string provinceName,
            PaginationParams pagination);
        Task<PagedResult<Province>> FilterByNameAsync(
            string provinceName,
            PaginationParams pagination);
    }
}
