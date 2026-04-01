using ePermitsApp.DTOs;

namespace ePermitsApp.Services.Interfaces
{
    public interface IAdminMaintenanceService
    {
        Task<IEnumerable<MaintenanceLookupItemDto>> GetLookupItemsAsync(
            string entityType,
            bool includeInactive,
            bool includeDeleted,
            string? search,
            string? applicationType);

        Task<MaintenanceLookupItemDto> SetActiveStatusAsync(string entityType, int id, bool isActive);
        Task<MaintenanceReferenceUsageDto> GetReferenceUsageAsync(string entityType, int id);
        Task<object> CreateItemAsync(string entityType, IDictionary<string, object?> values);
        Task<MaintenanceImportResultDto> ImportAsync(MaintenanceImportRequestDto request);
        Task<MemoryStream> GenerateTemplateAsync();
    }
}
