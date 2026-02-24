using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Services.Interfaces
{
    public interface IOwnershipTypeService
    {
        Task<IEnumerable<OwnershipType>> GetAllAsync();
        Task<OwnershipType?> GetByIdAsync(int id);
        Task<OwnershipType> CreateAsync(CreateOwnershipTypeDto dto);
        Task<bool> UpdateAsync(int id, UpdateOwnershipTypeDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<IEnumerable<OwnershipType>> GetByNameAsync(
            string ownershipTypeDesc,
            PaginationParams pagination);
        Task<PagedResult<OwnershipType>> FilterByNameAsync(
            string ownershipTypeDesc,
            PaginationParams pagination);
    }
}
