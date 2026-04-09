using System;

namespace ePermitsApp.Helpers
{
    public static class ApplicationWorkflowDefinitions
    {
        public static class PermitTypes
        {
            public const string BuildingPermit = "BuildingPermit";
            public const string CertificateOfOccupancy = "CertificateOfOccupancy";
        }

        public static class DepartmentIds
        {
            public const int OBO = 1;
            public const int CPDO = 2;
            public const int BFP = 3;
        }

        public static class OverallStatuses
        {
            public const string Draft = "Draft";
            public const string Submitted = "Submitted";
            public const string UnderReview = "Under Review";
            public const string Approved = "Approved";
            public const string Closed = "Closed";
        }

        public static class DepartmentStatuses
        {
            public const string InQueue = "In Queue";
            public const string UnderReview = "Under Review";
            public const string DeficiencyIssued = "Deficiency Issued";
            public const string ForRecheck = "For Recheck";
            public const string InspectionScheduled = "Inspection Scheduled";
            public const string Approved = "Approved";
        }

        public static readonly string[] OverallStatusOptions =
        {
            OverallStatuses.Draft,
            OverallStatuses.Submitted,
            OverallStatuses.UnderReview,
            OverallStatuses.Approved,
            OverallStatuses.Closed
        };

        public static readonly string[] DepartmentStatusOptions =
        {
            DepartmentStatuses.InQueue,
            DepartmentStatuses.UnderReview,
            DepartmentStatuses.DeficiencyIssued,
            DepartmentStatuses.ForRecheck,
            DepartmentStatuses.InspectionScheduled,
            DepartmentStatuses.Approved
        };

        public static IReadOnlyList<int> GetRequiredDepartmentIds(string permitType)
        {
            if (string.Equals(permitType, PermitTypes.BuildingPermit, StringComparison.OrdinalIgnoreCase))
            {
                return new[] { DepartmentIds.OBO, DepartmentIds.CPDO, DepartmentIds.BFP };
            }

            if (string.Equals(permitType, PermitTypes.CertificateOfOccupancy, StringComparison.OrdinalIgnoreCase))
            {
                return new[] { DepartmentIds.OBO, DepartmentIds.BFP };
            }

            return Array.Empty<int>();
        }

        public static bool IsValidDepartmentStatus(string status)
        {
            return DepartmentStatusOptions.Contains(status, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsValidOverallStatus(string status)
        {
            return OverallStatusOptions.Contains(status, StringComparer.OrdinalIgnoreCase);
        }

        public static bool CanTransitionDepartmentStatus(string currentStatus, string nextStatus)
        {
            if (string.Equals(currentStatus, nextStatus, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

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

        public static string DetermineOverallStatus(IEnumerable<string> departmentStatuses, string? currentOverallStatus = null)
        {
            if (string.Equals(currentOverallStatus, OverallStatuses.Closed, StringComparison.OrdinalIgnoreCase))
            {
                return OverallStatuses.Closed;
            }

            if (string.Equals(currentOverallStatus, OverallStatuses.Draft, StringComparison.OrdinalIgnoreCase))
            {
                return OverallStatuses.Draft;
            }

            var normalizedStatuses = departmentStatuses
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            if (normalizedStatuses.Count == 0)
            {
                return OverallStatuses.Submitted;
            }

            if (normalizedStatuses.All(s => string.Equals(s, DepartmentStatuses.Approved, StringComparison.OrdinalIgnoreCase)))
            {
                return OverallStatuses.Approved;
            }

            if (normalizedStatuses.All(s => string.Equals(s, DepartmentStatuses.InQueue, StringComparison.OrdinalIgnoreCase)))
            {
                return OverallStatuses.Submitted;
            }

            return OverallStatuses.UnderReview;
        }
    }
}
