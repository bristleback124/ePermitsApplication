using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Extensions;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class RequirementCategoryRepository : IRequirementCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public RequirementCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RequirementCategory>> GetAllAsync()
        {
            return await _context.RequirementCategorys
                .Include(r => r.RequirementClassification)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<RequirementCategory?> GetByIdAsync(int id)
        {
            return await _context.RequirementCategorys
                .Include(r => r.RequirementClassification)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(RequirementCategory reqCat)
        {
            await _context.RequirementCategorys.AddAsync(reqCat);
        }

        public void Update(RequirementCategory reqCat)
        {
            _context.RequirementCategorys.Update(reqCat);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<RequirementCategory?> GetByIdIncludingDeletedAsync(int id)
        {
            return await _context.RequirementCategorys
                .IgnoreQueryFilters()
                .Include(r => r.RequirementClassification)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<RequirementCategory>> GetByReqClassAsync(int reqClassId)
        {
            return await _context.RequirementCategorys
                .Include(r => r.RequirementClassification)
                .Where(r => r.ReqClassId == reqClassId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<RequirementCategory>> GetByReqClassIncludingDeletedAsync(int reqClassId)
        {
            return await _context.RequirementCategorys
                .IgnoreQueryFilters()
                .Where(r => r.ReqClassId == reqClassId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RequirementCategory>> GetByReqClassDescAsync(
            string reqClassDesc,
            PaginationParams pagination)
        {
            return await _context.RequirementCategorys
                .Include(r => r.RequirementClassification)
                .Where(r => r.RequirementClassification.ReqClassDesc.ToLower()
                    .Contains(reqClassDesc.ToLower()))
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();
        }

        public async Task<PagedResult<RequirementCategory>> FilterByReqClassDescAsync(
            string reqClassDesc,
            PaginationParams pagination)
        {
            var query = _context.RequirementCategorys
                .Include(r => r.RequirementClassification)
                .Where(r => r.RequirementClassification.ReqClassDesc.ToLower()
                    .Contains(reqClassDesc.ToLower()));

            var totalCount = await query.CountAsync();

            var items = await query
                .ApplySorting(pagination.OrderBy, pagination.IsDescending)
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();

            return new PagedResult<RequirementCategory>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }
    }
}
