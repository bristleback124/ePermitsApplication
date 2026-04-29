using ePermitsApp.Data;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task MigrateDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

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
                """);

            logger.LogInformation("Application formatted IDs normalized.");
        }
    }
}
