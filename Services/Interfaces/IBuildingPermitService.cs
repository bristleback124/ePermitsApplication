using ePermitsApp.DTOs;
using ePermitsApp.Entities.BuildingPermit;

namespace ePermitsApp.Services.Interfaces
{
    public interface IBuildingPermitService
    {
        Task<PagedResult<BuildingPermit>> GetAllAsync(PaginationParams pagination);
        Task<BuildingPermit?> GetByIdAsync(int id);
        Task<BuildingPermit> CreateAsync(BuildingPermitCreateDto dto);
    }
}
