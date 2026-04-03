using ePermits.Models;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IAuditTrailRepository
    {
        Task<IEnumerable<AuditTrail>> GetByApplicationIdAsync(int applicationId);
        Task<AuditTrail> AddAsync(AuditTrail auditTrail);
    }
}
