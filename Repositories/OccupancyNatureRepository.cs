using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Extensions;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class OccupancyNatureRepository : IOccupancyNatureRepository
    {
        private readonly ApplicationDbContext _context;

        public OccupancyNatureRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OccupancyNature>> GetAllAsync()
        {
            return await _context.OccupancyNatures.AsNoTracking().ToListAsync();
        }

        public async Task<OccupancyNature?> GetByIdAsync(int id)
        {
            return await _context.OccupancyNatures.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddAsync(OccupancyNature occupancyNature)
        {
            await _context.OccupancyNatures.AddAsync(occupancyNature);
        }

        public void Update(OccupancyNature occupancyNature)
        {
            _context.OccupancyNatures.Update(occupancyNature);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<OccupancyNature?> GetByIdIncludingDeletedAsync(int id)
        {
            return await _context.OccupancyNatures
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<IEnumerable<OccupancyNature>> GetByNameAsync(
            string occupancyNatureDesc,
            PaginationParams pagination)
        {
            return await _context.OccupancyNatures
                .Where(o => o.OccupancyNatureDesc.ToLower()
                    .Contains(occupancyNatureDesc.ToLower()))
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();
        }
        public async Task<PagedResult<OccupancyNature>> FilterByNameAsync(
            string occupancyNatureDesc,
            PaginationParams pagination)
        {
            var query = _context.OccupancyNatures
                .Where(o => o.OccupancyNatureDesc.ToLower()
                    .Contains(occupancyNatureDesc.ToLower()));

            var totalCount = await query.CountAsync();

            var items = await query
                .ApplySorting(pagination.OrderBy, pagination.IsDescending)
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();

            return new PagedResult<OccupancyNature>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }
    }
}
