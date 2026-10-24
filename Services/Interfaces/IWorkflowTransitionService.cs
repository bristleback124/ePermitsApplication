using ePermitsApp.Helpers;

namespace ePermitsApp.Services.Interfaces
{
    public interface IWorkflowTransitionService
    {
        IReadOnlyList<WorkflowTransition> GetAllowedTransitions(string currentStatus, IEnumerable<string> userRoles);
        (bool Success, string Message, WorkflowTransition? Transition) ValidateTransition(
            string currentStatus, IEnumerable<string> userRoles, string targetStatus, string? actingRole = null);
        string GetNextStepDescription(string status);
    }
}
