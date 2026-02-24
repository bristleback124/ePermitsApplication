using ePermits.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ePermits.Data
{
    public interface IUserRoleRepository
    {
        Task<UserRole?> GetByIdAsync(int id);
        Task<UserRole?> GetByDescriptionAsync(string description);
        Task<List<UserRole>> GetAllAsync();
        Task<UserRole> CreateAsync(UserRole role);
        Task<UserRole> UpdateAsync(UserRole role);
        Task<bool> DeleteAsync(int id);
    }
}