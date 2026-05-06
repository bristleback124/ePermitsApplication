using ePermitsApp.DTOs;

namespace ePermitsApp.Services.Interfaces
{
    public interface ISystemUtilitiesService
    {
        Task<ClearApplicationsResultDto> ClearAllApplicationsAsync(
            int performedByUserId,
            string performedByName,
            CancellationToken cancellationToken = default);
    }
}
