using System;

namespace ePermitsApp.Helpers
{
    public record WorkflowTransition(
        string FromStatus,
        string RequiredRole,
        string ToStatus,
        string ActionLabel,
        string ActionType // "forward" | "reset" | "close"
    );

    public static class ApplicationWorkflowDefinitions
    {
        public static class PermitTypes
        {
            public const string BuildingPermit = "BuildingPermit";
            public const string CertificateOfOccupancy = "CertificateOfOccupancy";
        }

        public static class Roles
        {
            public const string SuperAdmin = "superadmin";
            public const string User = "user"; // deprecated — kept for backwards compat
            public const string Applicant = "applicant";
            public const string Encoder = "encoder";
            public const string InitialReviewer = "initial-reviewer";
            public const string FeeAssessor = "fee-assessor";
            public const string FinalReviewer = "final-reviewer";
            public const string FinalApprover = "final-approver";
            public const string Executive = "executive";
            public const string SysAdmin = "sysadmin";
            public const string ReleasingOfficer = "releasing-officer";
        }

        public static class OverallStatuses
        {
            public const string Draft = "Draft";
            public const string Submitted = "Submitted";
            public const string UnderInitialReview = "Under Initial Review";
            public const string DeficiencyIssued = "Deficiency Issued";
            public const string ForFeeComputation = "For Fee Computation";
            public const string PaymentPending = "Payment Pending";
            public const string ForFinalReview = "For Final Review";
            public const string ForFinalApproval = "For Final Approval";
            public const string ApprovedForIssuance = "Approved - For Issuance";
            public const string ClosedIssued = "Closed - Issued";
            public const string ClosedRejected = "Closed - Rejected";
            public const string ClosedCancelled = "Closed - Cancelled";
        }

        public static readonly string[] AllStatuses =
        {
            OverallStatuses.Draft,
            OverallStatuses.Submitted,
            OverallStatuses.UnderInitialReview,
            OverallStatuses.DeficiencyIssued,
            OverallStatuses.ForFeeComputation,
            OverallStatuses.PaymentPending,
            OverallStatuses.ForFinalReview,
            OverallStatuses.ForFinalApproval,
            OverallStatuses.ApprovedForIssuance,
            OverallStatuses.ClosedIssued,
            OverallStatuses.ClosedRejected,
            OverallStatuses.ClosedCancelled
        };

        public static readonly string[] AllRoles =
        {
            Roles.SuperAdmin,
            Roles.User, // deprecated — kept for backwards compat validation
            Roles.Applicant,
            Roles.Encoder,
            Roles.InitialReviewer,
            Roles.FeeAssessor,
            Roles.FinalReviewer,
            Roles.FinalApprover,
            Roles.Executive,
            Roles.SysAdmin,
            Roles.ReleasingOfficer
        };

        public static readonly IReadOnlyList<WorkflowTransition> Transitions = new List<WorkflowTransition>
        {
            // === Initial Reviewer: forward (direct from Submitted — matches reference site) ===
            new(OverallStatuses.Submitted, Roles.InitialReviewer, OverallStatuses.ForFeeComputation, "Proceed to Fee Computation", "forward"),
            new(OverallStatuses.Submitted, Roles.InitialReviewer, OverallStatuses.DeficiencyIssued, "Issue Deficiency", "close"),

            // === Initial Reviewer: also handle Under Initial Review (for apps already in this state) ===
            new(OverallStatuses.UnderInitialReview, Roles.InitialReviewer, OverallStatuses.ForFeeComputation, "Proceed to Fee Computation", "forward"),
            new(OverallStatuses.UnderInitialReview, Roles.InitialReviewer, OverallStatuses.DeficiencyIssued, "Issue Deficiency", "close"),
            new(OverallStatuses.UnderInitialReview, Roles.InitialReviewer, OverallStatuses.Submitted, "Re-open", "reset"),

            // === Initial Reviewer: re-open deficiency ===
            new(OverallStatuses.DeficiencyIssued, Roles.InitialReviewer, OverallStatuses.Submitted, "Re-open", "reset"),

            // === Encoder + Applicant: resubmit after deficiency ===
            new(OverallStatuses.DeficiencyIssued, Roles.Encoder, OverallStatuses.Submitted, "Resubmit for Review", "forward"),
            new(OverallStatuses.DeficiencyIssued, Roles.Applicant, OverallStatuses.Submitted, "Resubmit for Review", "forward"),

            // === Fee Assessor: forward ===
            new(OverallStatuses.ForFeeComputation, Roles.FeeAssessor, OverallStatuses.PaymentPending, "Mark as Payment Pending", "forward"),
            new(OverallStatuses.PaymentPending, Roles.FeeAssessor, OverallStatuses.ForFinalReview, "Mark as Ready for Final Review", "forward"),

            // === Fee Assessor: reset ===
            new(OverallStatuses.ForFeeComputation, Roles.FeeAssessor, OverallStatuses.UnderInitialReview, "Re-open", "reset"),
            new(OverallStatuses.PaymentPending, Roles.FeeAssessor, OverallStatuses.UnderInitialReview, "Re-open", "reset"),

            // === Final Reviewer: forward ===
            new(OverallStatuses.ForFinalReview, Roles.FinalReviewer, OverallStatuses.ForFinalApproval, "Pass to Final Approval", "forward"),

            // === Final Reviewer: reset ===
            new(OverallStatuses.ForFinalReview, Roles.FinalReviewer, OverallStatuses.PaymentPending, "Re-open", "reset"),

            // === Final Approver: forward + close ===
            new(OverallStatuses.ForFinalApproval, Roles.FinalApprover, OverallStatuses.ApprovedForIssuance, "Approve for Issuance", "forward"),
            new(OverallStatuses.ForFinalApproval, Roles.FinalApprover, OverallStatuses.ClosedRejected, "Reject Application", "close"),
            new(OverallStatuses.ForFinalApproval, Roles.FinalApprover, OverallStatuses.ClosedCancelled, "Cancel Application", "close"),

            // === Final Approver: reset ===
            new(OverallStatuses.ForFinalApproval, Roles.FinalApprover, OverallStatuses.ForFinalReview, "Re-open", "reset"),

            // === Releasing Officer: issue permit (final step) + reset ===
            new(OverallStatuses.ApprovedForIssuance, Roles.ReleasingOfficer, OverallStatuses.ClosedIssued, "Mark as Issued", "close"),
            new(OverallStatuses.ApprovedForIssuance, Roles.ReleasingOfficer, OverallStatuses.ForFinalApproval, "Re-open", "reset"),

            // === Final Approver: re-open closed ===
            new(OverallStatuses.ClosedRejected, Roles.FinalApprover, OverallStatuses.ForFinalApproval, "Re-open", "reset"),
            new(OverallStatuses.ClosedCancelled, Roles.FinalApprover, OverallStatuses.ForFinalApproval, "Re-open", "reset"),

            // === SuperAdmin: issue permit + re-open closed (override authority) ===
            new(OverallStatuses.ApprovedForIssuance, Roles.SuperAdmin, OverallStatuses.ClosedIssued, "Issue Permit", "close"),
            new(OverallStatuses.ClosedRejected, Roles.SuperAdmin, OverallStatuses.ForFinalApproval, "Re-open", "reset"),
            new(OverallStatuses.ClosedCancelled, Roles.SuperAdmin, OverallStatuses.ForFinalApproval, "Re-open", "reset"),
        };

        public static readonly IReadOnlyDictionary<string, string> NextStepDescriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { OverallStatuses.Draft, "Complete and submit your application" },
            { OverallStatuses.Submitted, "Awaiting initial review" },
            { OverallStatuses.UnderInitialReview, "Under initial review" },
            { OverallStatuses.DeficiencyIssued, "Application returned — please address deficiencies and resubmit" },
            { OverallStatuses.ForFeeComputation, "Awaiting fee assessment" },
            { OverallStatuses.PaymentPending, "Awaiting payment confirmation" },
            { OverallStatuses.ForFinalReview, "Under final review" },
            { OverallStatuses.ForFinalApproval, "Awaiting final approval" },
            { OverallStatuses.ApprovedForIssuance, "Ready for permit issuance" },
            { OverallStatuses.ClosedIssued, "Permit issued" },
            { OverallStatuses.ClosedRejected, "Application rejected" },
            { OverallStatuses.ClosedCancelled, "Application cancelled" },
        };

        public static bool IsValidStatus(string status)
        {
            return AllStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsValidRole(string role)
        {
            return AllRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsTerminalStatus(string status)
        {
            return string.Equals(status, OverallStatuses.ClosedIssued, StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, OverallStatuses.ClosedRejected, StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, OverallStatuses.ClosedCancelled, StringComparison.OrdinalIgnoreCase);
        }

        // ---- Kept for backward compatibility (department review) ----
        // These are deprecated but preserved so existing code that references them
        // continues to compile until fully removed in a later spec.

        public static class DepartmentIds
        {
            public const int OBO = 1;
            public const int CPDO = 2;
            public const int BFP = 3;
        }

        [Obsolete("Replaced by role-based workflow. Stop creating new department reviews.")]
        public static class DepartmentStatuses
        {
            public const string InQueue = "In Queue";
            public const string UnderReview = "Under Review";
            public const string DeficiencyIssued = "Deficiency Issued";
            public const string ForRecheck = "For Recheck";
            public const string InspectionScheduled = "Inspection Scheduled";
            public const string Approved = "Approved";
        }

        [Obsolete("Replaced by role-based workflow.")]
        public static readonly string[] DepartmentStatusOptions =
        {
            DepartmentStatuses.InQueue,
            DepartmentStatuses.UnderReview,
            DepartmentStatuses.DeficiencyIssued,
            DepartmentStatuses.ForRecheck,
            DepartmentStatuses.InspectionScheduled,
            DepartmentStatuses.Approved
        };

        [Obsolete("Replaced by role-based workflow.")]
        public static bool IsValidDepartmentStatus(string status)
        {
            return DepartmentStatusOptions.Contains(status, StringComparer.OrdinalIgnoreCase);
        }

        [Obsolete("Replaced by role-based workflow. Use Transitions table instead.")]
        public static IReadOnlyList<int> GetRequiredDepartmentIds(string permitType)
        {
            if (string.Equals(permitType, PermitTypes.BuildingPermit, StringComparison.OrdinalIgnoreCase))
                return new[] { DepartmentIds.OBO, DepartmentIds.CPDO, DepartmentIds.BFP };

            if (string.Equals(permitType, PermitTypes.CertificateOfOccupancy, StringComparison.OrdinalIgnoreCase))
                return new[] { DepartmentIds.OBO, DepartmentIds.BFP };

            return Array.Empty<int>();
        }

        [Obsolete("Replaced by role-based workflow. Use WorkflowTransitionService instead.")]
        public static bool CanTransitionDepartmentStatus(string currentStatus, string nextStatus)
        {
            if (string.Equals(currentStatus, nextStatus, StringComparison.OrdinalIgnoreCase))
                return true;

            return currentStatus switch
            {
                DepartmentStatuses.InQueue => nextStatus == DepartmentStatuses.UnderReview,
                DepartmentStatuses.UnderReview => nextStatus == DepartmentStatuses.DeficiencyIssued
                    || nextStatus == DepartmentStatuses.InspectionScheduled
                    || nextStatus == DepartmentStatuses.Approved,
                DepartmentStatuses.DeficiencyIssued => nextStatus == DepartmentStatuses.ForRecheck,
                DepartmentStatuses.ForRecheck => nextStatus == DepartmentStatuses.UnderReview
                    || nextStatus == DepartmentStatuses.InspectionScheduled,
                DepartmentStatuses.InspectionScheduled => nextStatus == DepartmentStatuses.Approved,
                DepartmentStatuses.Approved => false,
                _ => false
            };
        }

        [Obsolete("Replaced by role-based workflow. Status is now set explicitly, not computed.")]
        public static string DetermineOverallStatus(IEnumerable<string> departmentStatuses, string? currentOverallStatus = null)
        {
            if (string.Equals(currentOverallStatus, OverallStatuses.ClosedIssued, StringComparison.OrdinalIgnoreCase)
                || string.Equals(currentOverallStatus, OverallStatuses.ClosedRejected, StringComparison.OrdinalIgnoreCase)
                || string.Equals(currentOverallStatus, OverallStatuses.ClosedCancelled, StringComparison.OrdinalIgnoreCase))
                return currentOverallStatus!;

            if (string.Equals(currentOverallStatus, OverallStatuses.Draft, StringComparison.OrdinalIgnoreCase))
                return OverallStatuses.Draft;

            return OverallStatuses.Submitted;
        }
    }
}
