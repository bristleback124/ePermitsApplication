namespace ePermitsApp.Constants
{
    /// Shared limits for DTO validation and DB columns.
    /// Keep in sync with the frontend's src/lib/validation-limits.ts.
    public static class ValidationLimits
    {
        public const int UsernameMinLength = 3;
        public const int UsernameMaxLength = 100;
        public const int EmailMaxLength = 100;
        public const int NameMaxLength = 100;

        // Permit amounts. Columns are decimal(18,2) for money and decimal(18,4) for areas
        // (ApplicationDbContext). MaxCost (~₱10 trillion) is the largest 2-decimal amount JS numbers
        // hold exactly (15 significant digits), so the wizard can't silently round centavos.
        public const string MaxCost = "9999999999999.99";
        public const string MaxArea = "999999999.9999";
    }
}
