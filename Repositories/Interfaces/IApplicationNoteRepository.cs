using ePermits.Models;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IApplicationNoteRepository
    {
        Task<IEnumerable<ApplicationNote>> GetByApplicationIdAsync(int applicationId);
        Task<IEnumerable<ApplicationNote>> GetVisibleByApplicationIdAsync(int applicationId);
        Task<ApplicationNote?> GetByIdAsync(int id);
        Task<ApplicationNote> AddAsync(ApplicationNote note);
        Task UpdateAsync(ApplicationNote note);
    }
}
