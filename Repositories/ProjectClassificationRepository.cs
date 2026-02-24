using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Extensions;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class ProjectClassificationRepository : IProjectClassificationRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectClassificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectClassification>> GetAllAsync()
        {
            return await _context.ProjectClassifications.AsNoTracking().ToListAsync();
        }

        public async Task<ProjectClassification?> GetByIdAsync(int id)
        {
            return await _context.ProjectClassifications.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(ProjectClassification projectClassification)
        {
            await _context.ProjectClassifications.AddAsync(projectClassification);
        }

        public void Update(ProjectClassification projectClassification)
        {
            _context.ProjectClassifications.Update(projectClassification);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<ProjectClassification?> GetByIdIncludingDeletedAsync(int id)
        {
            return await _context.ProjectClassifications
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<ProjectClassification>> GetByNameAsync(
            string projectClassDesc,
            PaginationParams pagination)
        {
            return await _context.ProjectClassifications
                .Where(p => p.ProjectClassDesc.ToLower()
                    .Contains(projectClassDesc.ToLower()))
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();
        }
        public async Task<PagedResult<ProjectClassification>> FilterByNameAsync(
            string projectClassDesc,
            PaginationParams pagination)
        {
            var query = _context.ProjectClassifications
                .Where(p => p.ProjectClassDesc.ToLower()
                    .Contains(projectClassDesc.ToLower()));

            var totalCount = await query.CountAsync();

            var items = await query
                .ApplySorting(pagination.OrderBy, pagination.IsDescending)
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();

            return new PagedResult<ProjectClassification>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }
    }
}
