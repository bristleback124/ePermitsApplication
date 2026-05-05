using ePermitsApp.Data;
using ePermitsApp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ePermitsApp.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task MigrateDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            var appIdSettings = scope.ServiceProvider
                .GetRequiredService<IOptions<ApplicationIdSettings>>()
                .Value;

            var pending = await db.Database.GetPendingMigrationsAsync();
            if (pending.Any())
            {
                logger.LogInformation("Applying {Count} pending migration(s)...", pending.Count());
                await db.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("Database is up to date. No pending migrations.");
            }

            logger.LogInformation("Using database: {Database}", db.Database.GetDbConnection().Database);

            logger.LogInformation(
                "ApplicationIdSettings: BuildingPermit (offset={BpOffset}, year={BpYear}); CertificateOfOccupancy (offset={CoOffset}, year={CoYear}). Normalizer skips rows in offset years.",
                appIdSettings.BuildingPermit.LegacySequenceOffset,
                appIdSettings.BuildingPermit.LegacyYear,
                appIdSettings.CertificateOfOccupancy.LegacySequenceOffset,
                appIdSettings.CertificateOfOccupancy.LegacyYear);

            await db.Database.ExecuteSqlRawAsync(
                """
                ;WITH RankedApplications AS (
                    SELECT
                        Id,
                        Type,
                        CreatedAt,
                        ROW_NUMBER() OVER (
                            PARTITION BY Type, YEAR(CreatedAt)
                            ORDER BY CreatedAt, Id
                        ) AS SequenceNumber
                    FROM Applications
                    WHERE Status <> 'Draft'
                        AND NOT (Type = 'BuildingPermit'         AND YEAR(CreatedAt) = @bpYear AND @bpYear > 0)
                        AND NOT (Type = 'CertificateOfOccupancy' AND YEAR(CreatedAt) = @coYear AND @coYear > 0)
                )
                UPDATE app
                SET FormattedId =
                    expected.ExpectedFormattedId
                FROM Applications app
                INNER JOIN (
                    SELECT
                        ranked.Id,
                        CASE
                            WHEN ranked.Type = 'BuildingPermit' THEN 'BP'
                            WHEN ranked.Type = 'CertificateOfOccupancy' THEN 'CO'
                            ELSE ranked.Type
                        END
                        + '-01-'
                        + RIGHT('0' + CAST(YEAR(ranked.CreatedAt) % 100 AS varchar(2)), 2)
                        + '-'
                        + RIGHT('0' + CAST(MONTH(ranked.CreatedAt) AS varchar(2)), 2)
                        + '-'
                        + RIGHT('0000' + CAST(ranked.SequenceNumber AS varchar(16)), 4) AS ExpectedFormattedId
                    FROM RankedApplications ranked
                ) expected ON expected.Id = app.Id
                WHERE app.FormattedId <> expected.ExpectedFormattedId;

                UPDATE Applications
                SET FormattedId = ''
                WHERE Status = 'Draft'
                    AND FormattedId <> '';
                """,
                new SqlParameter("@bpYear", appIdSettings.BuildingPermit.LegacyYear),
                new SqlParameter("@coYear", appIdSettings.CertificateOfOccupancy.LegacyYear));

            logger.LogInformation("Application formatted IDs normalized.");
        }
    }
}
