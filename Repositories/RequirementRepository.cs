using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Extensions;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class RequirementRepository : IRequirementRepository
    {
        private readonly ApplicationDbContext _context;

        public RequirementRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Requirement>> GetAllAsync()
        {
            return await _context.Requirements
                .Include(r => r.RequirementCategory)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Requirement?> GetByIdAsync(int id)
        {
            return await _context.Requirements
                .Include(r => r.RequirementCategory)
                    .ThenInclude(c => c.RequirementClassification)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task AddAsync(Requirement requirement)
        {
            await _context.Requirements.AddAsync(requirement);
        }

        public void Update(Requirement requirement)
        {
            _context.Requirements.Update(requirement);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Requirement?> GetByIdIncludingDeletedAsync(int id)
        {
            return await _context.Requirements
                .IgnoreQueryFilters()
                .Include(r => r.RequirementCategory)
                    .ThenInclude(c => c.RequirementClassification)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<PagedResult<Requirement>> FilterAsync(
            string? reqDesc,
            int? reqCatId,
            string? reqClassDesc,
            PaginationParams pagination)
        {
            var query = _context.Requirements
                .Include(r => r.RequirementCategory)
                    .ThenInclude(c => c.RequirementClassification)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(reqDesc))
                query = query.Where(r =>
                    r.ReqDesc.ToLower().Contains(reqDesc.ToLower()));

            if (reqCatId.HasValue)
                query = query.Where(r => r.ReqCatId == reqCatId);

            if (!string.IsNullOrWhiteSpace(reqClassDesc))
                query = query.Where(r =>
                    r.RequirementCategory.RequirementClassification.ReqClassDesc.ToLower()
                        .Contains(reqClassDesc.ToLower()));

            query = pagination.OrderBy?.ToLower() switch
            {
                "reqdesc" => pagination.IsDescending
                    ? query.OrderByDescending(r => r.ReqDesc)
                    : query.OrderBy(r => r.ReqDesc),

                _ => query.OrderBy(r => r.Id)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<Requirement>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<IEnumerable<Requirement>> GetDeletedByReqCatAsync(int reqCatId)
        {
            return await _context.Requirements
                .IgnoreQueryFilters()
                .Where(r => r.ReqCatId == reqCatId && r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Requirement>> GetByReqCatAsync(int reqCatId)
        {
            return await _context.Requirements
                .Where(r => r.ReqCatId == reqCatId)
                .ToListAsync();
        }
    }
}
