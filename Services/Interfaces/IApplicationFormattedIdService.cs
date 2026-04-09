using ePermits.Models;

namespace ePermitsApp.Services.Interfaces
{
    public interface IApplicationFormattedIdService
    {
        Task AssignFormattedIdAsync(Application application, CancellationToken cancellationToken = default);
    }
}
