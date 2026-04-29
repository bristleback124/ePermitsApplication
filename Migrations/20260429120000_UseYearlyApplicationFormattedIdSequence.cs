using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ePermitsApp.Data;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260429120000_UseYearlyApplicationFormattedIdSequence")]
    public partial class UseYearlyApplicationFormattedIdSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_Applications_FormattedId'
                        AND object_id = OBJECT_ID('Applications')
                )
                BEGIN
                    DROP INDEX [IX_Applications_FormattedId] ON [Applications];
                END;

                UPDATE Applications
                SET FormattedId = ''
                WHERE FormattedId IS NULL;

                DECLARE @FormattedIdDefaultConstraint nvarchar(128);
                SELECT @FormattedIdDefaultConstraint = dc.name
                FROM sys.default_constraints dc
                INNER JOIN sys.columns c ON c.default_object_id = dc.object_id
                WHERE dc.parent_object_id = OBJECT_ID('Applications')
                    AND c.name = 'FormattedId';

                IF @FormattedIdDefaultConstraint IS NOT NULL
                BEGIN
                    EXEC(N'ALTER TABLE [Applications] DROP CONSTRAINT [' + @FormattedIdDefaultConstraint + N']');
                END;

                ALTER TABLE [Applications]
                ALTER COLUMN [FormattedId] nvarchar(32) NOT NULL;

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

                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_Applications_FormattedId'
                        AND object_id = OBJECT_ID('Applications')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_Applications_FormattedId]
                    ON [Applications] ([FormattedId])
                    WHERE [FormattedId] IS NOT NULL AND [FormattedId] <> '';
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_Applications_FormattedId'
                        AND object_id = OBJECT_ID('Applications')
                )
                BEGIN
                    DROP INDEX [IX_Applications_FormattedId] ON [Applications];
                END;

                UPDATE Applications
                SET FormattedId = ''
                WHERE FormattedId IS NULL;

                DECLARE @FormattedIdDefaultConstraint nvarchar(128);
                SELECT @FormattedIdDefaultConstraint = dc.name
                FROM sys.default_constraints dc
                INNER JOIN sys.columns c ON c.default_object_id = dc.object_id
                WHERE dc.parent_object_id = OBJECT_ID('Applications')
                    AND c.name = 'FormattedId';

                IF @FormattedIdDefaultConstraint IS NOT NULL
                BEGIN
                    EXEC(N'ALTER TABLE [Applications] DROP CONSTRAINT [' + @FormattedIdDefaultConstraint + N']');
                END;

                ALTER TABLE [Applications]
                ALTER COLUMN [FormattedId] nvarchar(32) NOT NULL;

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

                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_Applications_FormattedId'
                        AND object_id = OBJECT_ID('Applications')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_Applications_FormattedId]
                    ON [Applications] ([FormattedId])
                    WHERE [FormattedId] IS NOT NULL AND [FormattedId] <> '';
                END;
                """);
        }
    }
}
