using ePermitsApp.DTOs;

namespace ePermitsApp.Services.Interfaces;

public interface IAdminEmailNotificationConfigService
{
    Task<AdminEmailNotificationConfigResponseDto> GetConfigAsync();
    Task UpdateConfigAsync(UpdateAdminEmailNotificationConfigDto dto);
    Task<List<string>> GetRecipientEmailsAsync(string applicationType);
}
