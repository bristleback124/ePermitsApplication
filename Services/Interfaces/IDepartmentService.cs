using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task<Department> CreateAsync(CreateDepartmentDto dto);
        Task<bool> UpdateAsync(int id, UpdateDepartmentDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<PagedResult<Department>> FilterAsync(
            string? departmentName,
            string? departmentCode,
            int? lguId,
            string? provinceName,
            PaginationParams pagination);       
    }
}
