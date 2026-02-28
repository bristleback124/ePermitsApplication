using ePermits.Data;
using ePermits.DTOs;
using ePermitsApp.Entities;

using ePermits.Models;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ePermits.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILGURepository _lguRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IUserProfileRepository userProfileRepository,
            IUserRoleRepository userRoleRepository,
            IDepartmentRepository departmentRepository,
            ILGURepository lguRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _userRoleRepository = userRoleRepository;
            _departmentRepository = departmentRepository;
            _lguRepository = lguRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            // Check if username exists
            if (await _userRepository.UsernameExistsAsync(registerDto.Username))
            {
                return null;
            }

            // Check if email exists
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
            {
                return null;
            }

            // Always assign "applicant" role for self-registration
            var role = await _userRoleRepository.GetByDescriptionAsync("applicant");
            if (role == null)
            {
                return null;
            }

            // Hash password
            string hashedPassword = HashPassword(registerDto.Password);

            // Create user with no LGU or Department (applicant defaults)
            var user = new User
            {
                Username = registerDto.Username,
                Password = hashedPassword,
                UserRoleId = role.Id,
                LGUId = null,
                DepartmentId = null,
                CreatedBy = registerDto.Username,
                CreatedAt = DateTime.UtcNow
            };

            // Save user first to get the Id
            User createdUser;
            try
            {
                createdUser = await _userRepository.CreateAsync(user);
            }
            catch (Exception ex)
            {
                // Log the actual exception
                Console.WriteLine($"User creation failed: {ex.Message}");
                return null;
            }

            // Create user profile with UserId set
            var userProfile = new UserProfile
            {
                UserId = createdUser.Id,
                FirstName = registerDto.FirstName,
                MiddleName = registerDto.MiddleName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                MobileNo = registerDto.MobileNo,
                CreatedBy = registerDto.Username,
                CreatedAt = DateTime.UtcNow
            };

            // Save user profile
            UserProfile createdProfile;
            try
            {
                createdProfile = await _userProfileRepository.CreateAsync(userProfile);

                // Update user with UserProfileId
                createdUser.UserProfileId = createdProfile.Id;
                await _userRepository.UpdateAsync(createdUser);
            }
            catch (Exception ex)
            {
                // Log the actual exception
                Console.WriteLine($"UserProfile creation failed: {ex.Message}");
                return null;
            }

            // Generate JWT token
            string token = GenerateJwtToken(createdUser.Id, createdUser.Username, role.UserRoleDesc, role.Id);

            Console.WriteLine($"Registration successful for user: {createdUser.Username}");

            // Return response
            return new AuthResponseDto
            {
                UserId = createdUser.Id,
                Username = createdUser.Username,
                Token = token,
                Role = role.UserRoleDesc,
                RoleId = role.Id,
                Profile = new UserProfileDto
                {
                    FirstName = createdProfile.FirstName,
                    MiddleName = createdProfile.MiddleName,
                    LastName = createdProfile.LastName,
                    Email = createdProfile.Email,
                    MobileNo = createdProfile.MobileNo
                }
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // Get user by email
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return null;
            }

            // Verify password
            if (!VerifyPassword(loginDto.Password, user.Password))
            {
                return null;
            }

            // Get user role
            var role = await _userRoleRepository.GetByIdAsync(user.UserRoleId);
            if (role == null)
            {
                return null;
            }

            // Get user profile
            var profile = await _userProfileRepository.GetByUserIdAsync(user.Id);
            if (profile == null)
            {
                return null;
            }

            // Generate JWT token
            string token = GenerateJwtToken(user.Id, user.Username, role.UserRoleDesc, role.Id);

            // Return response
            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Token = token,
                Role = role.UserRoleDesc,
                RoleId = role.Id,
                Profile = new UserProfileDto
                {
                    FirstName = profile.FirstName,
                    MiddleName = profile.MiddleName,
                    LastName = profile.LastName,
                    Email = profile.Email,
                    MobileNo = profile.MobileNo
                }
            };
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return await Task.FromResult(true);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }

        public string GenerateJwtToken(int userId, string username, string role, int roleId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("RoleId", roleId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpiryInHours"] ?? "24")),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            string hashOfInput = HashPassword(password);
            return hashOfInput == hashedPassword;
        }

        public async Task<AuthResponseDto?> GetUserInfoAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var role = await _userRoleRepository.GetByIdAsync(user.UserRoleId);
            if (role == null) return null;

            var profile = await _userProfileRepository.GetByUserIdAsync(user.Id);
            if (profile == null) return null;

            // Return AuthResponseDto WITHOUT token
            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Token = null, // No token here
                Role = role.UserRoleDesc,
                RoleId = role.Id,
                Profile = new UserProfileDto
                {
                    FirstName = profile.FirstName,
                    MiddleName = profile.MiddleName,
                    LastName = profile.LastName,
                    Email = profile.Email,
                    MobileNo = profile.MobileNo
                }
            };
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }

    }
}