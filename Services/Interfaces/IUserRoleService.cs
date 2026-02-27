using ePermits.Models;
using ePermitsApp.DTOs;

namespace ePermitsApp.Services.Interfaces
{
    public interface IUserRoleService
    {
        Task<IEnumerable<UserRole>> GetAllAsync();
        Task<UserRole?> GetByIdAsync(int id);
        Task<UserRole?> GetByDescriptionAsync(string description);
        Task<UserRole> CreateAsync(CreateUserRoleDto dto);
        Task<bool> UpdateAsync(int id, UpdateUserRoleDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
