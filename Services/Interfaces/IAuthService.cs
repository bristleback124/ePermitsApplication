using ePermits.DTOs;
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
    }
}