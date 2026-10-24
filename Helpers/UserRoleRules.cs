using ePermits.Models;
using Roles = ePermitsApp.Helpers.ApplicationWorkflowDefinitions.Roles;

namespace ePermitsApp.Helpers
{
    /// <summary>
    /// Multi-role users: which roles a user holds and which combinations are allowed.
    /// A user's roles are their UserRoleAssignments plus Users.UserRoleId (the primary role), so a user
    /// with no assignment rows still has their primary role.
    /// </summary>
    public static class UserRoleRules
    {
        /// <summary>Office workflow roles, in workflow order. Any subset may be combined on one account.</summary>
        public static readonly IReadOnlyList<string> CombinableRoles = new[]
        {
            Roles.Encoder,
            Roles.InitialReviewer,
            Roles.TechnicalReviewer,
            Roles.FeeAssessor,
            Roles.FinalReviewer,
            Roles.FinalApprover,
            Roles.ReleasingOfficer,
        };

        /// <summary>Roles an account can only hold on their own.</summary>
        public static readonly IReadOnlyList<string> ExclusiveRoles = new[]
        {
            Roles.Applicant,
            Roles.Executive,
            Roles.SysAdmin,
            Roles.SuperAdmin,
            Roles.User, // deprecated
            "admin", // backwards compat
        };

        public static IReadOnlyList<string> GetRoleNames(User? user)
        {
            if (user == null) return Array.Empty<string>();

            var names = user.RoleAssignments
                .Select(a => a.UserRole?.UserRoleDesc)
                .Append(user.UserRole?.UserRoleDesc)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r!)
                .Distinct(StringComparer.OrdinalIgnoreCase);

            return Order(names);
        }

        public static bool HasRole(User? user, string role)
        {
            return GetRoleNames(user).Contains(role, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>Combinable roles first in workflow order, then anything else.</summary>
        public static IReadOnlyList<string> Order(IEnumerable<string> roleNames)
        {
            return roleNames
                .OrderBy(r => RoleOrder(r))
                .ThenBy(r => r, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static int RoleOrder(string role)
        {
            for (var i = 0; i < CombinableRoles.Count; i++)
            {
                if (string.Equals(CombinableRoles[i], role, StringComparison.OrdinalIgnoreCase)) return i;
            }
            return CombinableRoles.Count;
        }

        public static (bool Ok, string Message) ValidateCombination(IReadOnlyCollection<string> roleNames)
        {
            if (roleNames.Count == 0)
                return (false, "At least one role is required");

            if (roleNames.Count == 1)
                return (true, string.Empty);

            var exclusive = roleNames.FirstOrDefault(r => ExclusiveRoles.Contains(r, StringComparer.OrdinalIgnoreCase));
            if (exclusive != null)
                return (false, $"The '{exclusive}' role can't be combined with other roles");

            return (true, string.Empty);
        }

        /// <summary>The single exclusive role if present, else the first combinable role in workflow order.</summary>
        public static string PickPrimary(IReadOnlyCollection<string> roleNames)
        {
            var exclusive = roleNames.FirstOrDefault(r => ExclusiveRoles.Contains(r, StringComparer.OrdinalIgnoreCase));
            return exclusive ?? Order(roleNames).First();
        }
    }
}
