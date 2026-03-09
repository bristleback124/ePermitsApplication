using ePermitsApp.DTOs;

namespace ePermitsApp.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationDtoShort>> GetApplicationsByUserIdAsync(int userId);
        Task<IEnumerable<ReviewerDashboardItemDto>> GetReviewerDashboardAsync();
        Task<ApplicationBuildingPermitDetailDto?> GetApplicationBuildingPermitById(int id);
        Task<ApplicationCoODetailDto?> GetApplicationCoOById(int id);
        Task<ApplicationStatusOptionsDto> GetStatusOptionsAsync();
        Task<IEnumerable<ReviewAssignableUserDto>> GetAssignableReviewersAsync(int departmentId);
        Task<(bool Success, string Message, ApplicationDepartmentReviewDto? Review)> AssignReviewerAsync(int applicationId, int departmentId, AssignApplicationReviewerDto dto);
        Task<(bool Success, string Message, ApplicationDepartmentReviewDto? Review)> UpdateDepartmentReviewStatusAsync(int applicationId, int departmentId, UpdateApplicationDepartmentReviewStatusDto dto);
        Task<(bool Success, string Message)> UpdateOverallStatusAsync(int applicationId, UpdateApplicationOverallStatusDto dto);
    }
}
