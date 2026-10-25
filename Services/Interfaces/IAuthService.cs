using ePermits.DTOs;
using ePermitsApp.DTOs;
using System.Threading.Tasks;

namespace ePermits.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<bool> ValidateTokenAsync(string token);
        Task<AuthResponseDto?> GetUserInfoAsync(int userId);
        string GenerateJwtToken(int userId, string username, IReadOnlyList<string> roles, int roleId);
        Task<bool> CheckEmailExistsAsync(string email);
        Task<(bool Success, string Message, RegisterApplicantResponseDto? Data)> RegisterApplicantAsync(RegisterApplicantDto dto, string registeredBy);
        Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordDto dto);

        /// Sets another user's password from User Management. Admins can't use it on themselves,
        /// and only a superadmin can change a superadmin's password.
        Task<(bool Success, bool Forbidden, string Message)> SetPasswordByAdminAsync(int targetUserId, string newPassword, int adminUserId, string adminRole);
    }
}