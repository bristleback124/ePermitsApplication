using ePermitsApp.DTOs;

namespace ePermitsApp.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationDtoShort>> GetApplicationsByUserIdAsync(int userId);
        Task<ApplicationDetailDto?> GetApplicationByIdAsync(int id);
    }
}
