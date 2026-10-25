namespace ePermitsApp.Helpers
{
    /// <summary>
    /// Timestamps are stored and sent to the client as UTC. Anything the server renders itself
    /// (emails, PDFs) has no browser to localize it, so it is converted to Philippine time here.
    /// </summary>
    public static class PhilippineTime
    {
        private static readonly TimeZoneInfo Zone = ResolveZone();

        public static DateTime Now => FromUtc(DateTime.UtcNow);

        /// <summary>Converts a UTC timestamp to Philippine time. Unspecified kinds are treated as UTC.</summary>
        public static DateTime FromUtc(DateTime value)
        {
            var utc = value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };
            return TimeZoneInfo.ConvertTimeFromUtc(utc, Zone);
        }

        private static TimeZoneInfo ResolveZone()
        {
            // IANA ids resolve on Linux and on Windows with ICU; the Windows id covers older hosts.
            foreach (var id in new[] { "Asia/Manila", "Singapore Standard Time" })
            {
                if (TimeZoneInfo.TryFindSystemTimeZoneById(id, out var zone))
                    return zone;
            }
            return TimeZoneInfo.CreateCustomTimeZone("PHT", TimeSpan.FromHours(8), "Philippine Time", "PHT");
        }
    }
}
