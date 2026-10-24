using ePermitsApp.Helpers;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class WorkflowTransitionService : IWorkflowTransitionService
    {
        // Transitions for any of the user's roles; a multi-role user gets the union, in workflow role order.
        public IReadOnlyList<WorkflowTransition> GetAllowedTransitions(string currentStatus, IEnumerable<string> userRoles)
        {
            var roles = userRoles.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
            if (string.IsNullOrWhiteSpace(currentStatus) || roles.Count == 0)
                return Array.Empty<WorkflowTransition>();

            return ApplicationWorkflowDefinitions.Transitions
                .Where(t => string.Equals(t.FromStatus, currentStatus, StringComparison.OrdinalIgnoreCase)
                         && roles.Contains(t.RequiredRole, StringComparer.OrdinalIgnoreCase))
                .OrderBy(t => UserRoleRules.RoleOrder(t.RequiredRole))
                .ToList();
        }

        public (bool Success, string Message, WorkflowTransition? Transition) ValidateTransition(
            string currentStatus, IEnumerable<string> userRoles, string targetStatus, string? actingRole = null)
        {
            var roles = userRoles.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();

            if (string.IsNullOrWhiteSpace(currentStatus))
                return (false, "Current status is required.", null);

            if (roles.Count == 0)
                return (false, "User role is required.", null);

            if (string.IsNullOrWhiteSpace(targetStatus))
                return (false, "Target status is required.", null);

            if (!ApplicationWorkflowDefinitions.IsValidStatus(targetStatus))
                return (false, $"'{targetStatus}' is not a recognized status.", null);

            if (!string.IsNullOrWhiteSpace(actingRole))
            {
                // The client said which role this action is taken as: it must be held and must own the move.
                if (!roles.Contains(actingRole, StringComparer.OrdinalIgnoreCase))
                    return (false, $"You don't hold the '{actingRole}' role.", null);
                roles = new List<string> { actingRole };
            }

            var allowed = GetAllowedTransitions(currentStatus, roles);
            var roleLabel = string.Join(", ", roles);

            if (allowed.Count == 0)
                return (false, $"Role '{roleLabel}' cannot act on applications with status '{currentStatus}'.", null);

            var match = allowed.FirstOrDefault(t =>
                string.Equals(t.ToStatus, targetStatus, StringComparison.OrdinalIgnoreCase));

            if (match == null)
                return (false, $"Transition from '{currentStatus}' to '{targetStatus}' is not allowed for role '{roleLabel}'.", null);

            return (true, $"Transition to '{targetStatus}' is valid.", match);
        }

        public string GetNextStepDescription(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return string.Empty;

            return ApplicationWorkflowDefinitions.NextStepDescriptions
                .TryGetValue(status, out var description)
                    ? description
                    : string.Empty;
        }
    }
}
