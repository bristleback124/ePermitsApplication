using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Extensions;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class LGURepository : ILGURepository
    {
        private readonly ApplicationDbContext _context;

        public LGURepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LGU>> GetAllAsync()
        {
            return await _context.LGUs
                .Include(l => l.Province)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LGU?> GetByIdAsync(int id)
        {
            return await _context.LGUs
                .Include(l => l.Province)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task AddAsync(LGU lgu)
        {
            await _context.LGUs.AddAsync(lgu);
        }

        public void Update(LGU lgu)
        {
            _context.LGUs.Update(lgu);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<LGU?> GetByIdIncludingDeletedAsync(int id)
        {
            return await _context.LGUs
                .IgnoreQueryFilters()
                .Include(l => l.Province)
                .FirstOrDefaultAsync(l => l.Id == id);
        }
                
        public async Task<IEnumerable<LGU>> GetByProvinceAsync(int provinceId)
        {
            return await _context.LGUs
                .Include(l => l.Province)
                .Where(l => l.ProvinceId == provinceId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<LGU>> GetByProvinceIncludingDeletedAsync(int provinceId)
        {
            return await _context.LGUs
                .IgnoreQueryFilters()
                .Where(l => l.ProvinceId == provinceId)
                .ToListAsync();
        }

        public async Task<IEnumerable<LGU>> GetByProvinceNameAsync(
            string provinceName,
            PaginationParams pagination)
        {
            return await _context.LGUs
                .Include(l => l.Province)
                .Where(l => l.Province.ProvinceName.ToLower()
                    .Contains(provinceName.ToLower()))
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();
        }               

        public async Task<PagedResult<LGU>> FilterByProvinceNameAsync(
            string provinceName,
            PaginationParams pagination)
        {
            var query = _context.LGUs
                .Include(l => l.Province)
                .Where(l => l.Province.ProvinceName.ToLower()
                    .Contains(provinceName.ToLower()));

            var totalCount = await query.CountAsync();

            var items = await query
                .ApplySorting(pagination.OrderBy, pagination.IsDescending)
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();

            return new PagedResult<LGU>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }
    }
}
