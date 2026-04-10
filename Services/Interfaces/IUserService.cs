using ePermits.DTOs;

namespace ePermits.Services
{
    public interface IUserService
    {
        Task<List<UserListItemDto>> GetAllUsersAsync();
        Task<UserListItemDto?> GetUserByIdAsync(int id);
        Task<(bool Success, string Message)> UpdateUserRoleAsync(int userId, UpdateUserAssignmentDto dto);
        Task<List<UserListItemDto>> SearchApplicantsAsync(string search);
    }
}
