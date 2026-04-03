using ePermits.Models;
using ePermitsApp.Data;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class AuditTrailRepository : IAuditTrailRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditTrailRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditTrail>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.AuditTrails
                .Where(a => a.ApplicationId == applicationId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<AuditTrail> AddAsync(AuditTrail auditTrail)
        {
            _context.AuditTrails.Add(auditTrail);
            await _context.SaveChangesAsync();
            return auditTrail;
        }
    }
}
