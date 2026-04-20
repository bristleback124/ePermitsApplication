using ePermits.Data;
using ePermits.DTOs;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;

using ePermits.Models;
using ePermitsApp.Models.EmailModels;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IUserProfileRepository userProfileRepository,
            IUserRoleRepository userRoleRepository,
            IDepartmentRepository departmentRepository,
            ILGURepository lguRepository,
            IConfiguration configuration,
            IEmailService emailService,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _userRoleRepository = userRoleRepository;
            _departmentRepository = departmentRepository;
            _lguRepository = lguRepository;
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
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
                MustChangePassword = createdUser.MustChangePassword,
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
                MustChangePassword = user.MustChangePassword,
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
                MustChangePassword = user.MustChangePassword,
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

        public async Task<(bool Success, string Message, RegisterApplicantResponseDto? Data)> RegisterApplicantAsync(RegisterApplicantDto dto, string registeredBy)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                return (false, "An account with this email already exists.", null);
            }

            // Generate username from email prefix
            var username = dto.Email.Split('@')[0].ToLowerInvariant();
            if (await _userRepository.UsernameExistsAsync(username))
            {
                username = $"{username}{DateTime.UtcNow.Ticks % 10000}";
            }

            // Generate temp password (8 chars alphanumeric)
            var tempPassword = GenerateTempPassword(8);

            var applicantRoleId = 3; // applicant role

            var user = new User
            {
                Username = username,
                Password = HashPassword(tempPassword),
                UserRoleId = applicantRoleId,
                MustChangePassword = true,
                CreatedBy = registeredBy,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);

            var userProfile = new UserProfile
            {
                UserId = createdUser.Id,
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName ?? string.Empty,
                LastName = dto.LastName,
                Email = dto.Email,
                MobileNo = dto.MobileNo,
                CreatedBy = registeredBy,
                CreatedAt = DateTime.UtcNow
            };

            await _userProfileRepository.CreateAsync(userProfile);

            createdUser.UserProfileId = userProfile.Id;
            await _userRepository.UpdateAsync(createdUser);

            var fullName = $"{dto.FirstName} {dto.LastName}".Trim();

            await SendApplicantCredentialsEmailsAsync(
                dto.Email, fullName, username, tempPassword, registeredBy);

            return (true, "Applicant registered successfully.", new RegisterApplicantResponseDto
            {
                UserId = createdUser.Id,
                FullName = fullName,
                Email = dto.Email
            });
        }

        private async Task SendApplicantCredentialsEmailsAsync(
            string applicantEmail,
            string applicantName,
            string username,
            string tempPassword,
            string encoderUsername)
        {
            try
            {
                var encoder = await _userRepository.GetByUsernameAsync(encoderUsername);
                var encoderEmail = encoder?.UserProfile?.Email;
                var encoderName = encoder?.UserProfile != null
                    ? $"{encoder.UserProfile.FirstName} {encoder.UserProfile.LastName}".Trim()
                    : encoderUsername;

                var model = new ApplicantCredentialsModel
                {
                    ApplicantName = applicantName,
                    EncoderName = string.IsNullOrWhiteSpace(encoderName) ? encoderUsername : encoderName,
                    Username = username,
                    TempPassword = tempPassword,
                    ApplicantEmail = applicantEmail,
                    RegisteredAt = DateTime.UtcNow
                };

                await _emailService.SendTemplatedEmailAsync(
                    applicantEmail,
                    "Welcome to ePermits — Your Account Credentials",
                    "ApplicantAccountCreated",
                    model);

                if (!string.IsNullOrWhiteSpace(encoderEmail))
                {
                    await _emailService.SendTemplatedEmailAsync(
                        encoderEmail,
                        $"Applicant Registered — {applicantName}",
                        "EncoderApplicantRegistered",
                        model);
                }
                else
                {
                    _logger.LogWarning(
                        "Encoder {EncoderUsername} has no profile email on file — skipping encoder confirmation email for applicant {ApplicantEmail}.",
                        encoderUsername,
                        applicantEmail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send credentials email after registering applicant {Email}",
                    applicantEmail);
            }
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found.");
            }

            if (!user.MustChangePassword)
            {
                // Voluntary flow: require and verify current password.
                if (string.IsNullOrEmpty(dto.CurrentPassword))
                {
                    return (false, "Current password is required.");
                }

                if (!VerifyPassword(dto.CurrentPassword, user.Password))
                {
                    return (false, "Current password is incorrect.");
                }
            }

            var newHash = HashPassword(dto.NewPassword);
            if (newHash == user.Password)
            {
                return (false, "New password must be different from the current password.");
            }

            user.Password = newHash;
            user.MustChangePassword = false;
            user.UpdatedBy = user.Username;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return (true, "Password changed successfully.");
        }

        private static string GenerateTempPassword(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            var random = new Random();
            return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

    }
}