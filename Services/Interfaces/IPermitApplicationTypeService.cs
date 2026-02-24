using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Services.Interfaces
{
    public interface IPermitApplicationTypeService
    {
        Task<IEnumerable<PermitApplicationType>> GetAllAsync();
        Task<PermitApplicationType?> GetByIdAsync(int id);
        Task<PermitApplicationType> CreateAsync(CreatePermitApplicationTypeDto dto);
        Task<bool> UpdateAsync(int id, UpdatePermitApplicationTypeDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<IEnumerable<PermitApplicationType>> GetByNameAsync(
            string permitAppTypeDesc,
            PaginationParams pagination);
        Task<PagedResult<PermitApplicationType>> FilterByNameAsync(
            string permitAppTypeDesc,
            PaginationParams pagination);
    }
}
