using System;
using ePermitsApp.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260429123000_AddReviewSubstatusesAndTechnicalReviewer")]
    public partial class AddReviewSubstatusesAndTechnicalReviewer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF COL_LENGTH('Applications', 'RequirementsReviewStatus') IS NULL
                BEGIN
                    ALTER TABLE [Applications]
                    ADD [RequirementsReviewStatus] nvarchar(50) NOT NULL
                    CONSTRAINT [DF_Applications_RequirementsReviewStatus] DEFAULT N'Review Not Started';
                END;

                IF COL_LENGTH('Applications', 'TechnicalPlansReviewStatus') IS NULL
                BEGIN
                    ALTER TABLE [Applications]
                    ADD [TechnicalPlansReviewStatus] nvarchar(50) NOT NULL
                    CONSTRAINT [DF_Applications_TechnicalPlansReviewStatus] DEFAULT N'Review Not Started';
                END;

                IF NOT EXISTS (SELECT 1 FROM [UserRoles] WHERE [Id] = 11 OR [UserRoleDesc] = 'technical-reviewer')
                BEGIN
                    SET IDENTITY_INSERT [UserRoles] ON;
                    INSERT INTO [UserRoles] ([Id], [UserRoleDesc], [CreatedBy], [CreatedAt])
                    VALUES (11, 'technical-reviewer', 'System', '2025-01-01T00:00:00.0000000Z');
                    SET IDENTITY_INSERT [UserRoles] OFF;
                END;

                DECLARE @TechnicalReviewerRoleId int = (SELECT TOP 1 [Id] FROM [UserRoles] WHERE [UserRoleDesc] = 'technical-reviewer');

                IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [Id] = 107 OR [Username] = 'technicalreviewer')
                   AND NOT EXISTS (SELECT 1 FROM [UserProfiles] WHERE [Id] = 107 OR [Email] = 'technicalreviewer@lgu.gov.ph')
                BEGIN
                    SET IDENTITY_INSERT [Users] ON;
                    INSERT INTO [Users] ([Id], [Username], [Password], [UserRoleId], [UserProfileId], [LGUId], [DepartmentId], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [MustChangePassword])
                    VALUES (107, 'technicalreviewer', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', @TechnicalReviewerRoleId, 107, 1, NULL, 'System', '2025-01-01T00:00:00.0000000Z', NULL, NULL, 0);
                    SET IDENTITY_INSERT [Users] OFF;

                    SET IDENTITY_INSERT [UserProfiles] ON;
                    INSERT INTO [UserProfiles] ([Id], [UserId], [FirstName], [MiddleName], [LastName], [Email], [MobileNo], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
                    VALUES (107, 107, 'Teresa', '', 'Villanueva', 'technicalreviewer@lgu.gov.ph', '09171000008', 'System', '2025-01-01T00:00:00.0000000Z', NULL, NULL);
                    SET IDENTITY_INSERT [UserProfiles] OFF;
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.Sql(
                """
                DELETE FROM [UserProfiles]
                WHERE [Id] = 107 AND [Email] = 'technicalreviewer@lgu.gov.ph';

                DELETE FROM [Users]
                WHERE [Id] = 107 AND [Username] = 'technicalreviewer';

                DELETE FROM [UserRoles]
                WHERE [Id] = 11 AND [UserRoleDesc] = 'technical-reviewer';

                IF COL_LENGTH('Applications', 'RequirementsReviewStatus') IS NOT NULL
                BEGIN
                    ALTER TABLE [Applications] DROP COLUMN [RequirementsReviewStatus];
                END;

                IF COL_LENGTH('Applications', 'TechnicalPlansReviewStatus') IS NOT NULL
                BEGIN
                    ALTER TABLE [Applications] DROP COLUMN [TechnicalPlansReviewStatus];
                END;
                """);
        }
    }
}
