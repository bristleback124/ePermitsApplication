using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Extensions;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class ProvinceRepository : IProvinceRepository
    {
        private readonly ApplicationDbContext _context;

        public ProvinceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Province>> GetAllAsync()
        {
            return await _context.Provinces.AsNoTracking().ToListAsync();
        }

        public async Task<Province?> GetByIdAsync(int id)
        {
            return await _context.Provinces.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Province province)
        {
            await _context.Provinces.AddAsync(province);
        }

        public void Update(Province province)
        {
            _context.Provinces.Update(province);
        }              

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Province?> GetByIdIncludingDeletedAsync(int id)
        {
            return await _context.Provinces
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<Province>> GetByNameAsync(
            string provinceName,
            PaginationParams pagination)
        {
            return await _context.Provinces
                .Where(p => p.ProvinceName.ToLower()
                    .Contains(provinceName.ToLower()))
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();
        }
        public async Task<PagedResult<Province>> FilterByNameAsync(
            string provinceName,
            PaginationParams pagination)
        {
            var query = _context.Provinces
                .Where(p => p.ProvinceName.ToLower()
                    .Contains(provinceName.ToLower()));

            var totalCount = await query.CountAsync();

            var items = await query
                .ApplySorting(pagination.OrderBy, pagination.IsDescending)
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();

            return new PagedResult<Province>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }
    }
}
