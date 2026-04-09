using ePermitsApp.DTOs;
using ePermitsApp.Entities.BuildingPermit;

namespace ePermitsApp.Services.Interfaces
{
    public interface IBuildingPermitService
    {
        Task<PagedResult<BuildingPermit>> GetAllAsync(PaginationParams pagination);
        Task<BuildingPermit?> GetByIdAsync(int id);
        Task<BuildingPermit> CreateAsync(BuildingPermitCreateDto dto, bool saveAsDraft = false);
        Task<BuildingPermitEditDto?> GetEditByApplicationIdAsync(int applicationId);
        Task<BuildingPermitEditDto?> GetFormByApplicationIdAsync(int applicationId);
        Task<(bool Success, string Message, BuildingPermit? BuildingPermit)> UpdateByApplicationIdAsync(int applicationId, BuildingPermitUpdateDto dto, bool saveAsDraft = false);
    }
}
