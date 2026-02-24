using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IProvinceRepository
    {
        Task<IEnumerable<Province>> GetAllAsync();
        Task<Province?> GetByIdAsync(int id);
        Task AddAsync(Province province);
        void Update(Province province);
        Task<bool> SaveChangesAsync();
        Task<Province?> GetByIdIncludingDeletedAsync(int id);
        Task<IEnumerable<Province>> GetByNameAsync(
            string provinceName,
            PaginationParams pagination);        
        Task<PagedResult<Province>> FilterByNameAsync(
            string provinceName,
            PaginationParams pagination);
    }
}
