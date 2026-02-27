using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Extensions;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class RequirementClassificationRepository : IRequirementClassificationRepository
    {
        private readonly ApplicationDbContext _context;

        public RequirementClassificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RequirementClassification>> GetAllAsync()
        {
            return await _context.RequirementClassifications.AsNoTracking().ToListAsync();
        }

        public async Task<RequirementClassification?> GetByIdAsync(int id)
        {
            return await _context.RequirementClassifications.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(RequirementClassification reqClass)
        {
            await _context.RequirementClassifications.AddAsync(reqClass);
        }

        public void Update(RequirementClassification reqClass)
        {
            _context.RequirementClassifications.Update(reqClass);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<RequirementClassification?> GetByIdIncludingDeletedAsync(int id)
        {
            return await _context.RequirementClassifications
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<IEnumerable<RequirementClassification>> GetByNameAsync(
            string reqClassDesc,
            PaginationParams pagination)
        {
            return await _context.RequirementClassifications
                .Where(r => r.ReqClassDesc.ToLower()
                    .Contains(reqClassDesc.ToLower()))
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();
        }
        public async Task<PagedResult<RequirementClassification>> FilterByNameAsync(
            string reqClassDesc,
            PaginationParams pagination)
        {
            var query = _context.RequirementClassifications
                .Where(r => r.ReqClassDesc.ToLower()
                    .Contains(reqClassDesc.ToLower()));

            var totalCount = await query.CountAsync();

            var items = await query
                .ApplySorting(pagination.OrderBy, pagination.IsDescending)
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();

            return new PagedResult<RequirementClassification>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<IEnumerable<RequirementClassification>> GetAllWithHierarchyAsync()
        {
            return await _context.RequirementClassifications
                .Include(rc => rc.RequirementCategorys)
                    .ThenInclude(cat => cat.Requirements)
                .AsNoTracking()
                .OrderBy(rc => rc.Id)
                .ToListAsync();
        }
    }
}
