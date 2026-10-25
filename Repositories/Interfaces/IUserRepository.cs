using ePermits.Models;

namespace ePermits.Data
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetAllAsync();
        Task<List<User>> GetAssignableReviewersAsync(int departmentId);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        /// Saves a tracked user (loaded by GetByIdAsync) with its role assignments replaced by roleIds.
        Task<User> UpdateWithRolesAsync(User user, IReadOnlyCollection<int> roleIds);
        Task<bool> DeleteAsync(int id);
    }
}
