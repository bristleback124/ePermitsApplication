using ePermitsApp.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260429124500_SeedTechnicalReviewerUser")]
    public partial class SeedTechnicalReviewerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DECLARE @TechnicalReviewerRoleId int = (SELECT TOP 1 [Id] FROM [UserRoles] WHERE [UserRoleDesc] = 'technical-reviewer');

                IF @TechnicalReviewerRoleId IS NOT NULL
                    AND NOT EXISTS (SELECT 1 FROM [Users] WHERE [Id] = 107 OR [Username] = 'technicalreviewer')
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
                """);
        }
    }
}
