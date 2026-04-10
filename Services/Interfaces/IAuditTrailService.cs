using ePermitsApp.DTOs;

namespace ePermitsApp.Services.Interfaces
{
    public interface IAuditTrailService
    {
        Task<IEnumerable<AuditTrailDto>> GetByApplicationIdAsync(int applicationId);
        Task LogAsync(int applicationId, string actionType, string action, string? details, int userId, string userName);
        Task<(bool Success, string Message)> UpdateDetailsAsync(int auditId, string newDetails, int editedByUserId, string editedByName);
        Task<IEnumerable<AuditTrailDto>> GetAdminActivityAsync();
    }
}
