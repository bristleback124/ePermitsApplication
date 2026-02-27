using ePermitsApp.DTOs;

namespace ePermitsApp.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationDtoShort>> GetApplicationsByUserIdAsync(int userId);
        Task<ApplicationBuildingPermitDetailDto?> GetApplicationBuildingPermitById(int id);
    }
}
