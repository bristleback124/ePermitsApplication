namespace ePermitsApp.Constants
{
    /// Shared length limits for DTO validation and DB columns.
    /// Keep in sync with the frontend's src/lib/validation-limits.ts.
    public static class ValidationLimits
    {
        public const int UsernameMinLength = 3;
        public const int UsernameMaxLength = 20;
        public const int EmailMaxLength = 100;
    }
}
