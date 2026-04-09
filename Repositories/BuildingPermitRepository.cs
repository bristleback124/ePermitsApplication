using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities.BuildingPermit;
using ePermitsApp.Extensions;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class BuildingPermitRepository : IBuildingPermitRepository
    {
        private readonly ApplicationDbContext _context;

        public BuildingPermitRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<BuildingPermit>> GetAllAsync(PaginationParams pagination)
        {
            var query = _context.BuildingPermits
                .Include(b => b.AppInfo)
                .Include(b => b.DesignProf)
                .Include(b => b.TechDoc)
                .Include(b => b.SupportingDoc)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query
                .ApplySorting(pagination.OrderBy, pagination.IsDescending)
                .Paginate(pagination)
                .ToListAsync();

            return new PagedResult<BuildingPermit>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<BuildingPermit?> GetByIdAsync(int id)
        {
            return await _context.BuildingPermits
                .Include(b => b.Application)
                .Include(b => b.AppInfo)
                .Include(b => b.DesignProf)
                .Include(b => b.TechDoc)
                .Include(b => b.SupportingDoc)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<BuildingPermit?> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.BuildingPermits
                .Include(b => b.Application)
                    .ThenInclude(a => a!.DepartmentReviews)
                .Include(b => b.AppInfo)
                .Include(b => b.DesignProf)
                .Include(b => b.TechDoc)
                .Include(b => b.SupportingDoc)
                .FirstOrDefaultAsync(b => b.ApplicationId == applicationId);
        }

        public async Task<BuildingPermit?> GetDraftByUserIdAsync(int userId)
        {
            return await _context.BuildingPermits
                .Include(b => b.Application)
                .Include(b => b.AppInfo)
                .Include(b => b.DesignProf)
                .Include(b => b.TechDoc)
                .Include(b => b.SupportingDoc)
                .FirstOrDefaultAsync(b =>
                    b.Application != null &&
                    b.Application.UserId == userId &&
                    b.Application.Type == Helpers.ApplicationWorkflowDefinitions.PermitTypes.BuildingPermit &&
                    b.Application.Status == Helpers.ApplicationWorkflowDefinitions.OverallStatuses.Draft);
        }

        public async Task AddAsync(BuildingPermit buildingPermit)
        {
            await _context.BuildingPermits.AddAsync(buildingPermit);
        }

        public void Update(BuildingPermit buildingPermit)
        {
            _context.BuildingPermits.Update(buildingPermit);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
