using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities.CoOApp;
using ePermitsApp.Extensions;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class CoOAppRepository : ICoOAppRepository
    {
        private readonly ApplicationDbContext _context;

        public CoOAppRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<CoOApp>> GetAllAsync(PaginationParams pagination)
        {
            var query = _context.CoOApps
                .Include(c => c.CoOAppProf)
                .Include(c => c.CoOAppReqDoc)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query
                .ApplySorting(pagination.OrderBy, pagination.IsDescending)
                .Paginate(pagination)
                .ToListAsync();

            return new PagedResult<CoOApp>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<CoOApp?> GetByIdAsync(int id)
        {
            return await _context.CoOApps
                .Include(c => c.Application)
                .Include(c => c.CoOAppProf)
                .Include(c => c.CoOAppReqDoc)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CoOApp?> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.CoOApps
                .Include(c => c.Application)
                .Include(c => c.CoOAppProf)
                .Include(c => c.CoOAppReqDoc)
                .FirstOrDefaultAsync(c => c.ApplicationId == applicationId);
        }

        public async Task AddAsync(CoOApp coOApp)
        {
            await _context.CoOApps.AddAsync(coOApp);
        }

        public void Update(CoOApp coOApp)
        {
            _context.CoOApps.Update(coOApp);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
