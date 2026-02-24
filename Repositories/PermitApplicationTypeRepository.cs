using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Extensions;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class PermitApplicationTypeRepository : IPermitApplicationTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public PermitApplicationTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PermitApplicationType>> GetAllAsync()
        {
            return await _context.PermitApplicationTypes.AsNoTracking().ToListAsync();
        }

        public async Task<PermitApplicationType?> GetByIdAsync(int id)
        {
            return await _context.PermitApplicationTypes.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(PermitApplicationType permitApplicationType)
        {
            await _context.PermitApplicationTypes.AddAsync(permitApplicationType);
        }

        public void Update(PermitApplicationType permitApplicationType)
        {
            _context.PermitApplicationTypes.Update(permitApplicationType);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<PermitApplicationType?> GetByIdIncludingDeletedAsync(int id)
        {
            return await _context.PermitApplicationTypes
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<PermitApplicationType>> GetByNameAsync(
            string permitAppTypeDesc,
            PaginationParams pagination)
        {
            return await _context.PermitApplicationTypes
                .Where(p => p.PermitAppTypeDesc.ToLower()
                    .Contains(permitAppTypeDesc.ToLower()))
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();
        }
        public async Task<PagedResult<PermitApplicationType>> FilterByNameAsync(
            string permitAppTypeDesc,
            PaginationParams pagination)
        {
            var query = _context.PermitApplicationTypes
                .Where(p => p.PermitAppTypeDesc.ToLower()
                    .Contains(permitAppTypeDesc.ToLower()));

            var totalCount = await query.CountAsync();

            var items = await query
                .ApplySorting(pagination.OrderBy, pagination.IsDescending)
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();

            return new PagedResult<PermitApplicationType>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }
    }
}
