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
        string GenerateJwtToken(int userId, string username, string role, int roleId);
        Task<bool> CheckEmailExistsAsync(string email);
        Task<(bool Success, string Message, RegisterApplicantResponseDto? Data)> RegisterApplicantAsync(RegisterApplicantDto dto, string registeredBy);
        Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    }
}