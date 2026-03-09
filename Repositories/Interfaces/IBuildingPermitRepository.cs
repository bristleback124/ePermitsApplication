using ePermitsApp.Entities.BuildingPermit;
using ePermitsApp.DTOs;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IBuildingPermitRepository
    {
        Task<PagedResult<BuildingPermit>> GetAllAsync(PaginationParams pagination);
        Task<BuildingPermit?> GetByIdAsync(int id);
        Task<BuildingPermit?> GetByApplicationIdAsync(int applicationId);
        Task AddAsync(BuildingPermit buildingPermit);
        void Update(BuildingPermit buildingPermit);
        Task<bool> SaveChangesAsync();
    }
}
