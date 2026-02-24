using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Extensions;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class BarangayRepository : IBarangayRepository
    {
        private readonly ApplicationDbContext _context;

        public BarangayRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Barangay>> GetAllAsync()
        {
            return await _context.Barangays
                .Include(b => b.LGU)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Barangay?> GetByIdAsync(int id)
        { 
            return await _context.Barangays
                .Include(b => b.LGU)
                    .ThenInclude(l => l.Province)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task AddAsync(Barangay barangay)
        {
            await _context.Barangays.AddAsync(barangay);
        }     

        public void Update(Barangay barangay)
        {
            _context.Barangays.Update(barangay);
        }    

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }   
        
        public async Task<Barangay?> GetByIdIncludingDeletedAsync(int id)
        {
            return await _context.Barangays
                .IgnoreQueryFilters()
                .Include(b => b.LGU)
                    .ThenInclude(l => l.Province)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<PagedResult<Barangay>> FilterAsync(
            string? barangayName,
            int? lguId,
            string? provinceName,
            PaginationParams pagination)
        {
            var query = _context.Barangays
                .Include(b => b.LGU)
                    .ThenInclude(l => l.Province)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(barangayName))
                query = query.Where(b =>
                    b.BarangayName.ToLower().Contains(barangayName.ToLower()));

            if (lguId.HasValue)
                query = query.Where(b => b.LGUId == lguId);

            if (!string.IsNullOrWhiteSpace(provinceName))
                query = query.Where(b =>
                    b.LGU.Province.ProvinceName.ToLower()
                        .Contains(provinceName.ToLower()));

            query = pagination.OrderBy?.ToLower() switch
            {
                "barangayname" => pagination.IsDescending
                    ? query.OrderByDescending(b => b.BarangayName)
                    : query.OrderBy(b => b.BarangayName),

                _ => query.OrderBy(b => b.Id)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .AsNoTracking()
                .ToListAsync();
            
            return new PagedResult<Barangay>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<IEnumerable<Barangay>> GetDeletedByLGUAsync(int lguId)
        {
            return await _context.Barangays
                .IgnoreQueryFilters()
                .Where(b => b.LGUId == lguId && b.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Barangay>> GetByLGUAsync(int lguId)
        {
            return await _context.Barangays
                .Where(b => b.LGUId == lguId)
                .ToListAsync();
        }
    }
}
