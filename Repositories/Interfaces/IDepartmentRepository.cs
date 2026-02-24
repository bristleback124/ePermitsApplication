using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);                
        Task AddAsync(Department department);
        void Update(Department department);
        Task<bool> SaveChangesAsync();
        Task<Department?> GetByIdIncludingDeletedAsync(int id);
        Task<PagedResult<Department>> FilterAsync(
            string? departmentName,
            string? departmentCode,
            int? lguId,
            string? provinceName,
            PaginationParams pagination);
        Task<IEnumerable<Department>> GetDeletedByLGUAsync(int lguId);
        Task<IEnumerable<Department>> GetByLGUAsync(int lguId);
    }
}
