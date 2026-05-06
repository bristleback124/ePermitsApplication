using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Services
{
    public class SystemUtilitiesService : ISystemUtilitiesService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SystemUtilitiesService> _logger;

        public SystemUtilitiesService(
            ApplicationDbContext context,
            ILogger<SystemUtilitiesService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ClearApplicationsResultDto> ClearAllApplicationsAsync(
            int performedByUserId,
            string performedByName,
            CancellationToken cancellationToken = default)
        {
            _logger.LogWarning(
                "ClearAllApplications initiated by UserId={UserId} ({UserName})",
                performedByUserId, performedByName);

            var result = new ClearApplicationsResultDto();
            using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

            // Delete in FK-safe order. AuditTrail uses Restrict to Application; cascade FKs would
            // handle the rest, but explicit deletion gives accurate per-table counts.
            result.DeletedCounts["AuditTrails"]                  = await _context.AuditTrails.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["ApplicationNotes"]             = await _context.ApplicationNotes.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["ApplicationDepartmentReviews"] = await _context.ApplicationDepartmentReviews.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["MessageRecipientStates"]       = await _context.MessageRecipientStates.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["Messages"]                     = await _context.Messages.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["PaymentDocuments"]             = await _context.PaymentDocuments.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["IssuedPermitDocuments"]        = await _context.IssuedPermitDocuments.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["ClearanceDocuments"]           = await _context.ClearanceDocuments.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["BuildingPermitTechDocs"]       = await _context.BuildingPermitTechDocs.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["BuildingPermitSupportingDocs"] = await _context.BuildingPermitSupportingDocs.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["BuildingPermitDesignProfs"]    = await _context.BuildingPermitDesignProfs.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["BuildingPermitAppInfos"]       = await _context.BuildingPermitAppInfos.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["BuildingPermits"]              = await _context.BuildingPermits.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["CoOAppProfs"]                  = await _context.CoOAppProfs.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["CoOAppReqDocs"]                = await _context.CoOAppReqDocs.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["CoOApps"]                      = await _context.CoOApps.ExecuteDeleteAsync(cancellationToken);
            result.DeletedCounts["Applications"]                 = await _context.Applications.ExecuteDeleteAsync(cancellationToken);

            await _context.Database.ExecuteSqlRawAsync(@"
                DBCC CHECKIDENT ('Applications', RESEED, 0);
                DBCC CHECKIDENT ('Messages', RESEED, 0);
                DBCC CHECKIDENT ('MessageRecipientStates', RESEED, 0);
                DBCC CHECKIDENT ('AuditTrails', RESEED, 0);
                DBCC CHECKIDENT ('ApplicationNotes', RESEED, 0);
                DBCC CHECKIDENT ('ApplicationDepartmentReviews', RESEED, 0);
                DBCC CHECKIDENT ('PaymentDocuments', RESEED, 0);
                DBCC CHECKIDENT ('IssuedPermitDocuments', RESEED, 0);
                DBCC CHECKIDENT ('ClearanceDocuments', RESEED, 0);
                DBCC CHECKIDENT ('BuildingPermits', RESEED, 0);
                DBCC CHECKIDENT ('BuildingPermitAppInfos', RESEED, 0);
                DBCC CHECKIDENT ('BuildingPermitDesignProfs', RESEED, 0);
                DBCC CHECKIDENT ('BuildingPermitTechDocs', RESEED, 0);
                DBCC CHECKIDENT ('BuildingPermitSupportingDocs', RESEED, 0);
                DBCC CHECKIDENT ('CoOApps', RESEED, 0);
                DBCC CHECKIDENT ('CoOAppProfs', RESEED, 0);
                DBCC CHECKIDENT ('CoOAppReqDocs', RESEED, 0);
            ", cancellationToken);

            await tx.CommitAsync(cancellationToken);

            result.TotalRowsDeleted = result.DeletedCounts.Values.Sum();
            result.CompletedAt = DateTime.UtcNow;

            _logger.LogWarning(
                "ClearAllApplications completed. TotalRowsDeleted={Total} Counts={@Counts}",
                result.TotalRowsDeleted, result.DeletedCounts);

            return result;
        }
    }
}
