namespace ePermitsApp.Helpers
{
    public static class MaintenanceApplicationScopes
    {
        public const string BuildingPermit = "BuildingPermit";
        public const string CertificateOfOccupancy = "CertificateOfOccupancy";
        public const string Both = "Both";

        public static readonly string[] All =
        {
            BuildingPermit,
            CertificateOfOccupancy,
            Both
        };

        public static bool IsValid(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return All.Any(scope => string.Equals(scope, value, StringComparison.OrdinalIgnoreCase));
        }

        public static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Both;

            return All.FirstOrDefault(scope => string.Equals(scope, value, StringComparison.OrdinalIgnoreCase))
                ?? Both;
        }

        public static bool Matches(string scope, string? applicationType)
        {
            var normalizedScope = Normalize(scope);
            if (string.IsNullOrWhiteSpace(applicationType))
                return true;

            if (string.Equals(normalizedScope, Both, StringComparison.OrdinalIgnoreCase))
                return true;

            return string.Equals(normalizedScope, applicationType, StringComparison.OrdinalIgnoreCase);
        }
    }
}
