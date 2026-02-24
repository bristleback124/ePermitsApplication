using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IPermitApplicationTypeRepository
    {
        Task<IEnumerable<PermitApplicationType>> GetAllAsync();
        Task<PermitApplicationType?> GetByIdAsync(int id);
        Task AddAsync(PermitApplicationType permitApplicationType);
        void Update(PermitApplicationType permitApplicationType);
        Task<bool> SaveChangesAsync();
        Task<PermitApplicationType?> GetByIdIncludingDeletedAsync(int id);
        Task<IEnumerable<PermitApplicationType>> GetByNameAsync(
            string permitAppTypeDesc,
            PaginationParams pagination);
        Task<PagedResult<PermitApplicationType>> FilterByNameAsync(
            string permitAppTypeDesc,
            PaginationParams pagination);
    }
}
