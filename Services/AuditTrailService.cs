using AutoMapper;
using ePermits.Models;
using ePermitsApp.DTOs;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class AuditTrailService : IAuditTrailService
    {
        private readonly IAuditTrailRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AuditTrailService> _logger;

        public AuditTrailService(
            IAuditTrailRepository repository,
            IMapper mapper,
            ILogger<AuditTrailService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
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
    }
}
