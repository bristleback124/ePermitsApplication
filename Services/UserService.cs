using ePermits.Data;
using ePermits.DTOs;
using ePermitsApp.Repositories.Interfaces;

namespace ePermits.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ILGURepository _lguRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public UserService(
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            ILGURepository lguRepository,
            IDepartmentRepository departmentRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _lguRepository = lguRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<List<UserListItemDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => MapToDto(u)).ToList();
        }

        public async Task<UserListItemDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;
            return MapToDto(user);
        }

        public async Task<(bool Success, string Message)> UpdateUserRoleAsync(int userId, UpdateUserAssignmentDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found");

            var targetRole = await _userRoleRepository.GetByIdAsync(dto.UserRoleId);
            if (targetRole == null)
                return (false, "Invalid role");

            var roleName = targetRole.UserRoleDesc.ToLower();

            if (roleName == "user")
            {
                // "user" role requires LGU and Department
                if (!dto.LGUId.HasValue || !dto.DepartmentId.HasValue)
                    return (false, "LGU and Department are required for the 'user' role");

                var lgu = await _lguRepository.GetByIdAsync(dto.LGUId.Value);
                if (lgu == null)
                    return (false, "Invalid LGU");

                var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId.Value);
                if (department == null)
                    return (false, "Invalid Department");

                user.LGUId = dto.LGUId;
                user.DepartmentId = dto.DepartmentId;
            }
            else
            {
                // "admin" and "applicant" roles don't need LGU/Department
                user.LGUId = null;
                user.DepartmentId = null;
            }

            user.UserRoleId = dto.UserRoleId;
            user.UpdatedAt = System.DateTime.UtcNow;
            user.UpdatedBy = "admin";

            await _userRepository.UpdateAsync(user);
            return (true, "User role updated successfully");
        }

        public async Task<List<UserListItemDto>> SearchApplicantsAsync(string search)
        {
            var users = await _userRepository.GetAllAsync();
            var applicants = users
                .Where(u => string.Equals(u.UserRole?.UserRoleDesc, "applicant", System.StringComparison.OrdinalIgnoreCase))
                .Where(u =>
                {
                    if (string.IsNullOrWhiteSpace(search)) return true;
                    var term = search.ToLowerInvariant();
                    var name = u.UserProfile != null
                        ? $"{u.UserProfile.FirstName} {u.UserProfile.LastName}".ToLowerInvariant()
                        : u.Username.ToLowerInvariant();
                    var email = u.UserProfile?.Email?.ToLowerInvariant() ?? "";
                    return name.Contains(term) || email.Contains(term) || u.Username.ToLowerInvariant().Contains(term);
                })
                .Take(20)
                .Select(u => MapToDto(u))
                .ToList();

            return applicants;
        }

        private static UserListItemDto MapToDto(Models.User user)
        {
            var profile = user.UserProfile;
            return new UserListItemDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.UserRole?.UserRoleDesc ?? "unknown",
                RoleId = user.UserRoleId,
                FullName = profile != null
                    ? $"{profile.FirstName} {profile.MiddleName} {profile.LastName}".Trim()
                    : user.Username,
                Email = profile?.Email ?? "",
                MobileNo = profile?.MobileNo ?? "",
                LguName = user.LGU?.LGUName,
                LguId = user.LGUId,
                DepartmentName = user.Department?.DepartmentName,
                DepartmentId = user.DepartmentId
            };
        }
    }
}
