using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Extensions;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class OwnershipTypeRepository : IOwnershipTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public OwnershipTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OwnershipType>> GetAllAsync()
        {
            return await _context.OwnershipTypes.AsNoTracking().ToListAsync();
        }

        public async Task<OwnershipType?> GetByIdAsync(int id)
        {
            return await _context.OwnershipTypes.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddAsync(OwnershipType ownershipType)
        {
            await _context.OwnershipTypes.AddAsync(ownershipType);
        }

        public void Update(OwnershipType ownershipType)
        {
            _context.OwnershipTypes.Update(ownershipType);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<OwnershipType?> GetByIdIncludingDeletedAsync(int id)
        {
            return await _context.OwnershipTypes
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<IEnumerable<OwnershipType>> GetByNameAsync(
            string ownershipTypeDesc,
            PaginationParams pagination)
        {
            return await _context.OwnershipTypes
                .Where(o => o.OwnershipTypeDesc.ToLower()
                    .Contains(ownershipTypeDesc.ToLower()))
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();
        }
        public async Task<PagedResult<OwnershipType>> FilterByNameAsync(
            string ownershipTypeDesc,
            PaginationParams pagination)
        {
            var query = _context.OwnershipTypes
                .Where(o => o.OwnershipTypeDesc.ToLower()
                    .Contains(ownershipTypeDesc.ToLower()));

            var totalCount = await query.CountAsync();

            var items = await query
                .ApplySorting(pagination.OrderBy, pagination.IsDescending)
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();

            return new PagedResult<OwnershipType>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }
    }
}
