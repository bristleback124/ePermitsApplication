using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IOwnershipTypeRepository
    {
        Task<IEnumerable<OwnershipType>> GetAllAsync();
        Task<OwnershipType?> GetByIdAsync(int id);
        Task AddAsync(OwnershipType ownershipType);
        void Update(OwnershipType ownershipType);
        Task<bool> SaveChangesAsync();
        Task<OwnershipType?> GetByIdIncludingDeletedAsync(int id);
        Task<IEnumerable<OwnershipType>> GetByNameAsync(
            string ownershipTypeDesc,
            PaginationParams pagination);
        Task<PagedResult<OwnershipType>> FilterByNameAsync(
            string ownershipTypeDesc,
            PaginationParams pagination);
    }
}
