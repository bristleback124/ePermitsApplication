using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApplicationFormattedIdFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FormattedId",
                table: "Applications",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
                    + RIGHT('00' + CAST(ranked.SequenceNumber AS varchar(3)), 3)
                FROM Applications app
                INNER JOIN RankedApplications ranked ON ranked.Id = app.Id;
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
                UPDATE Applications
                SET FormattedId =
                    CASE
                        WHEN Type = 'BuildingPermit' THEN 'BP'
                        WHEN Type = 'CertificateOfOccupancy' THEN 'CO'
                        ELSE Type
                    END
                    + '-'
                    + CAST(YEAR(CreatedAt) AS varchar(4))
                    + '-'
                    + RIGHT('000' + CAST(Id AS varchar(3)), 3);
                """);

            migrationBuilder.AlterColumn<string>(
                name: "FormattedId",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);
        }
    }
}
