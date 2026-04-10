using ePermitsApp.Helpers;

namespace ePermitsApp.Services.Interfaces
{
    public interface IWorkflowTransitionService
    {
        IReadOnlyList<WorkflowTransition> GetAllowedTransitions(string currentStatus, string userRole);
        (bool Success, string Message) ValidateTransition(string currentStatus, string userRole, string targetStatus);
        string GetNextStepDescription(string status);
    }
}
