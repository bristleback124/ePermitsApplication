using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface ILGURepository
    {
        Task<IEnumerable<LGU>> GetAllAsync();
        Task<LGU?> GetByIdAsync(int id);
        Task AddAsync(LGU lgu);
        void Update(LGU lgu);
        Task<bool> SaveChangesAsync();
        Task<LGU?> GetByIdIncludingDeletedAsync(int id);
        Task<IEnumerable<LGU>> GetByProvinceAsync(int provinceId);
        Task<IEnumerable<LGU>> GetByProvinceIncludingDeletedAsync(int provinceId);
        Task<IEnumerable<LGU>> GetByProvinceNameAsync(
            string provinceName, 
            PaginationParams pagination);
        Task<PagedResult<LGU>> FilterByProvinceNameAsync(
            string provinceName,
            PaginationParams pagination);
    }
}
