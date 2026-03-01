using ePermitsApp.DTOs;

namespace ePermitsApp.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationDtoShort>> GetApplicationsByUserIdAsync(int userId);
        Task<IEnumerable<ReviewerDashboardItemDto>> GetReviewerDashboardAsync();
        Task<ApplicationBuildingPermitDetailDto?> GetApplicationBuildingPermitById(int id);
        Task<ApplicationCoODetailDto?> GetApplicationCoOById(int id);
    }
}
