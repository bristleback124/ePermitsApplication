using ePermitsApp.Helpers;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class WorkflowTransitionService : IWorkflowTransitionService
    {
        public IReadOnlyList<WorkflowTransition> GetAllowedTransitions(string currentStatus, string userRole)
        {
            if (string.IsNullOrWhiteSpace(currentStatus) || string.IsNullOrWhiteSpace(userRole))
                return Array.Empty<WorkflowTransition>();

            return ApplicationWorkflowDefinitions.Transitions
                .Where(t => string.Equals(t.FromStatus, currentStatus, StringComparison.OrdinalIgnoreCase)
                         && string.Equals(t.RequiredRole, userRole, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public (bool Success, string Message) ValidateTransition(string currentStatus, string userRole, string targetStatus)
        {
            if (string.IsNullOrWhiteSpace(currentStatus))
                return (false, "Current status is required.");

            if (string.IsNullOrWhiteSpace(userRole))
                return (false, "User role is required.");

            if (string.IsNullOrWhiteSpace(targetStatus))
                return (false, "Target status is required.");

            if (!ApplicationWorkflowDefinitions.IsValidStatus(targetStatus))
                return (false, $"'{targetStatus}' is not a recognized status.");

            var allowed = GetAllowedTransitions(currentStatus, userRole);

            if (allowed.Count == 0)
                return (false, $"Role '{userRole}' cannot act on applications with status '{currentStatus}'.");

            var match = allowed.FirstOrDefault(t =>
                string.Equals(t.ToStatus, targetStatus, StringComparison.OrdinalIgnoreCase));

            if (match == null)
                return (false, $"Transition from '{currentStatus}' to '{targetStatus}' is not allowed for role '{userRole}'.");

            return (true, $"Transition to '{targetStatus}' is valid.");
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
