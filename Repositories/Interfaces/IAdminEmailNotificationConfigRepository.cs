using ePermitsApp.Entities;

namespace ePermitsApp.Repositories.Interfaces;

public interface IAdminEmailNotificationConfigRepository
{
    Task<List<AdminEmailNotificationConfig>> GetByApplicationTypeAsync(string applicationType);
    Task<List<AdminEmailNotificationConfig>> GetAllAsync();
    Task ReplaceForApplicationTypeAsync(string applicationType, List<int> userIds);
}
