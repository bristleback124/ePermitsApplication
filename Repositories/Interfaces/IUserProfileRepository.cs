using ePermits.Models;
using System.Threading.Tasks;

namespace ePermits.Data
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByIdAsync(int id);
        Task<UserProfile?> GetByUserIdAsync(int userId);
        Task<UserProfile> CreateAsync(UserProfile profile);
        Task<UserProfile> UpdateAsync(UserProfile profile);
        Task<bool> DeleteAsync(int id);
    }
}