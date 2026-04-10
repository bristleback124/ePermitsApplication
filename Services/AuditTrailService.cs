using AutoMapper;
using ePermits.Models;
using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Helpers;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Services
{
    public class AuditTrailService : IAuditTrailService
    {
        private readonly IAuditTrailRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AuditTrailService> _logger;
        private readonly ApplicationDbContext _context;

        public AuditTrailService(
            IAuditTrailRepository repository,
            IMapper mapper,
            ILogger<AuditTrailService> logger,
            ApplicationDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<AuditTrailDto>> GetByApplicationIdAsync(int applicationId)
        {
            var auditTrails = await _repository.GetByApplicationIdAsync(applicationId);
            return _mapper.Map<IEnumerable<AuditTrailDto>>(auditTrails);
        }

        public async Task LogAsync(int applicationId, string actionType, string action, string? details, int userId, string userName)
        {
            try
            {
                var auditTrail = new AuditTrail
                {
                    ApplicationId = applicationId,
                    ActionType = actionType,
                    Action = action,
                    Details = details,
                    PerformedByUserId = userId,
                    PerformedByName = userName,
                    CreatedAt = DateTime.UtcNow
                };

                await _repository.AddAsync(auditTrail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log audit trail for application {ApplicationId}, action {ActionType}", applicationId, actionType);
            }
        }
        public async Task<(bool Success, string Message)> UpdateDetailsAsync(int auditId, string newDetails, int editedByUserId, string editedByName)
        {
            var entry = await _context.Set<AuditTrail>().FindAsync(auditId);
            if (entry == null)
                return (false, "Audit entry not found.");

            var previousDetails = entry.Details;
            entry.Details = newDetails;
            await _context.SaveChangesAsync();

            // Log the edit itself
            await LogAsync(
                entry.ApplicationId,
                "audit_edit",
                $"Audit entry #{auditId} details edited by {editedByName}",
                $"Previous: {previousDetails ?? "(empty)"}",
                editedByUserId,
                editedByName);

            return (true, "Audit entry updated.");
        }

        public async Task<IEnumerable<AuditTrailDto>> GetAdminActivityAsync()
        {
            var superadminUserIds = await _context.Users
                .Where(u => u.UserRole != null &&
                    (u.UserRole.UserRoleDesc == ApplicationWorkflowDefinitions.Roles.SuperAdmin
                    || u.UserRole.UserRoleDesc == "admin"))
                .Select(u => u.Id)
                .ToListAsync();

            var entries = await _context.Set<AuditTrail>()
                .Where(a => superadminUserIds.Contains(a.PerformedByUserId))
                .OrderByDescending(a => a.CreatedAt)
                .Take(200)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AuditTrailDto>>(entries);
        }
    }
}
