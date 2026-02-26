using ePermits.Models;

namespace ePermits.Data
{
    public interface IApplicationRepository
    {
        Task<Application?> GetByIdAsync(int id);
        Task<Application?> GetByIdDetailedAsync(int id);
        Task<IEnumerable<Application>> GetAllAsync();
        Task<Application> AddAsync(Application application);
        Task UpdateAsync(Application application);
        Task DeleteAsync(int id);
        Task<IEnumerable<Application>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Application>> GetByUserIdDetailedAsync(int userId);
    }
}
