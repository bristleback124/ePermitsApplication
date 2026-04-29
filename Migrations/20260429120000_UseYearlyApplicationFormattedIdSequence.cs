using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class UseYearlyApplicationFormattedIdSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Applications_FormattedId",
                table: "Applications");

            migrationBuilder.Sql(
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
                    + RIGHT('0000' + CAST(ranked.SequenceNumber AS varchar(16)), 4)
                FROM Applications app
                INNER JOIN RankedApplications ranked ON ranked.Id = app.Id;

                UPDATE Applications
                SET FormattedId = ''
                WHERE Status = 'Draft';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_FormattedId",
                table: "Applications",
                column: "FormattedId",
                unique: true,
                filter: "[FormattedId] IS NOT NULL AND [FormattedId] <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Applications_FormattedId",
                table: "Applications");

            migrationBuilder.Sql(
                """
                ;WITH RankedApplications AS (
                    SELECT
                        Id,
                        Type,
                        CreatedAt,
                        ROW_NUMBER() OVER (
                            PARTITION BY Type, YEAR(CreatedAt), MONTH(CreatedAt)
                            ORDER BY CreatedAt, Id
                        ) AS SequenceNumber
                    FROM Applications
                    WHERE Status <> 'Draft'
                )
                UPDATE app
                SET FormattedId =
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
                    + RIGHT('00' + CAST(ranked.SequenceNumber AS varchar(16)), 3)
                FROM Applications app
                INNER JOIN RankedApplications ranked ON ranked.Id = app.Id;

                UPDATE Applications
                SET FormattedId = ''
                WHERE Status = 'Draft';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_FormattedId",
                table: "Applications",
                column: "FormattedId",
                unique: true,
                filter: "[FormattedId] IS NOT NULL AND [FormattedId] <> ''");
        }
    }
}
