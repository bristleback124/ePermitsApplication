using ePermits.Data;
using ePermits.DTOs;
using ePermitsApp.Repositories.Interfaces;
using ePermits.Models;
using ePermitsApp.Helpers;
using Roles = ePermitsApp.Helpers.ApplicationWorkflowDefinitions.Roles;

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

        public async Task<(bool Success, string Message)> UpdateUserRoleAsync(int userId, UpdateUserAssignmentDto dto, string actingAdminRole, int actingUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found");

            // Old clients send a single UserRoleId; new ones send UserRoleIds.
            var roleIds = (dto.UserRoleIds is { Count: > 0 }
                    ? dto.UserRoleIds
                    : dto.UserRoleId.HasValue ? new List<int> { dto.UserRoleId.Value } : new List<int>())
                .Distinct()
                .ToList();
            if (roleIds.Count == 0)
                return (false, "At least one role is required");

            var targetRoles = new List<UserRole>();
            foreach (var roleId in roleIds)
            {
                var role = await _userRoleRepository.GetByIdAsync(roleId);
                if (role == null)
                    return (false, "Invalid role");
                targetRoles.Add(role);
            }

            var roleNames = targetRoles.Select(r => r.UserRoleDesc).ToList();
            var (isValidCombination, combinationMessage) = UserRoleRules.ValidateCombination(roleNames);
            if (!isValidCombination)
                return (false, combinationMessage);

            // Only a superadmin can grant superadmin or change a superadmin's roles (mirrors SetPasswordByAdminAsync).
            var callerIsSuperAdmin = string.Equals(actingAdminRole, Roles.SuperAdmin, System.StringComparison.OrdinalIgnoreCase);
            var touchesSuperAdmin = UserRoleRules.HasRole(user, Roles.SuperAdmin)
                || roleNames.Contains(Roles.SuperAdmin, System.StringComparer.OrdinalIgnoreCase);
            if (touchesSuperAdmin && !callerIsSuperAdmin)
                return (false, "Only a superadmin can change superadmin roles");

            // A superadmin can't demote themselves (it could leave the system with no superadmin).
            if (userId == actingUserId
                && UserRoleRules.HasRole(user, Roles.SuperAdmin)
                && !roleNames.Contains(Roles.SuperAdmin, System.StringComparer.OrdinalIgnoreCase))
                return (false, "You can't remove your own superadmin role");

            var isDepartmentUser = roleNames.Count == 1
                && string.Equals(roleNames[0], Roles.User, System.StringComparison.OrdinalIgnoreCase);

            if (isDepartmentUser)
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
                // Other roles don't need LGU/Department
                user.LGUId = null;
                user.DepartmentId = null;
            }

            var primaryName = UserRoleRules.PickPrimary(roleNames);
            var primary = targetRoles.First(r => string.Equals(r.UserRoleDesc, primaryName, System.StringComparison.OrdinalIgnoreCase));
            user.UserRoleId = primary.Id;
            user.UserRole = primary;
            user.UpdatedAt = System.DateTime.UtcNow;
            user.UpdatedBy = "admin";

            await _userRepository.UpdateWithRolesAsync(user, roleIds);
            return (true, roleIds.Count == 1 ? "User role updated successfully" : "User roles updated successfully");
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
                        ? $"{u.UserProfile.FirstName} {u.UserProfile.MiddleName} {u.UserProfile.LastName}".ToLowerInvariant()
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
                Roles = UserRoleRules.GetRoleNames(user).ToList(),
                RoleIds = user.RoleAssignments.Select(a => a.UserRoleId).Append(user.UserRoleId).Distinct().ToList(),
                FullName = profile != null
                    ? string.IsNullOrWhiteSpace(profile.LastName)
                        ? profile.FirstName.Trim()
                        : $"{profile.FirstName} {profile.MiddleName} {profile.LastName}".Trim()
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
