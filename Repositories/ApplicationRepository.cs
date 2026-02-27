using ePermits.Models;
using ePermitsApp.Data;
using Microsoft.EntityFrameworkCore;

namespace ePermits.Data
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Application?> GetByIdAsync(int id)
        {
            return await _context.Applications.FindAsync(id);
        }

        public async Task<IEnumerable<Application>> GetAllAsync()
        {
            return await _context.Applications.ToListAsync();
        }

        public async Task<Application> AddAsync(Application application)
        {
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task UpdateAsync(Application application)
        {
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var application = await GetByIdAsync(id);
            if (application != null)
            {
                _context.Applications.Remove(application);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Application>> GetByUserIdAsync(int userId)
        {
            return await _context.Applications
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetByUserIdDetailedAsync(int userId)
        {
            return await _context.Applications
                .Include(a => a.BuildingPermit)
                .Include(a => a.CoOApp)
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Application?> GetByIdBuildingPermitDetailedAsync(int id)
        {
            return await _context.Applications
                .Include(a => a.User)
                    .ThenInclude(u => u!.UserProfile)
                .Include(a => a.BuildingPermit)
                    .ThenInclude(b => b!.AppInfo)
                .Include(a => a.BuildingPermit)
                    .ThenInclude(b => b!.TechDoc)
                .Include(a => a.BuildingPermit)
                    .ThenInclude(b => b!.PermitApplicationType)
                .Include(a => a.BuildingPermit)
                    .ThenInclude(b => b!.OccupancyNature)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Application?> GetByIdCoODetailedAsync(int id)
        {
            return await _context.Applications
                .Include(a => a.User)
                    .ThenInclude(u => u!.UserProfile)
                .Include(a => a.CoOApp)
                    .ThenInclude(c => c!.CoOAppProf)
                .Include(a => a.CoOApp)
                    .ThenInclude(c => c!.CoOAppReqDoc)
                .Include(a => a.CoOApp)
                    .ThenInclude(c => c!.OccupancyNature)
                .Include(a => a.CoOApp)
                    .ThenInclude(c => c!.Province)
                .Include(a => a.CoOApp)
                    .ThenInclude(c => c!.Lgu)
                .Include(a => a.CoOApp)
                    .ThenInclude(c => c!.Barangay)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
