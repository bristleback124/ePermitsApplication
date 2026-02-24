using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Services.Interfaces
{
    public interface ILGUService
    {
        Task<IEnumerable<LGU>> GetAllAsync();
        Task<LGU?> GetByIdAsync(int id);
        Task<LGU> CreateAsync(CreateLGUDto dto);
        Task<bool> UpdateAsync(int id, UpdateLGUDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<IEnumerable<LGU>> GetByProvinceAsync(int provinceId);
        Task<IEnumerable<LGU>> GetByProvinceNameAsync(
            string provinceName, 
            PaginationParams pagination);
        Task<PagedResult<LGU>> FilterByProvinceNameAsync(
            string provinceName,
            PaginationParams pagination);
    }
}
