using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddReleasingOfficerRoleAndIssuanceAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Accessories",
                table: "BuildingPermits",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "IssuedAt",
                table: "Applications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IssuedById",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                """
                IF NOT EXISTS (SELECT 1 FROM [UserRoles] WHERE [Id] = 12 OR [UserRoleDesc] = 'releasing-officer')
                BEGIN
                    SET IDENTITY_INSERT [UserRoles] ON;
                    INSERT INTO [UserRoles] ([Id], [UserRoleDesc], [CreatedBy], [CreatedAt])
                    VALUES (12, 'releasing-officer', 'System', '2025-01-01T00:00:00.0000000Z');
                    SET IDENTITY_INSERT [UserRoles] OFF;
                END;

                DECLARE @ReleasingOfficerRoleId int = (SELECT TOP 1 [Id] FROM [UserRoles] WHERE [UserRoleDesc] = 'releasing-officer');

                IF @ReleasingOfficerRoleId IS NOT NULL
                    AND NOT EXISTS (SELECT 1 FROM [Users] WHERE [Id] = 108 OR [Username] = 'releasingofficer')
                    AND NOT EXISTS (SELECT 1 FROM [UserProfiles] WHERE [Id] = 108 OR [Email] = 'releasingofficer@lgu.gov.ph')
                BEGIN
                    SET IDENTITY_INSERT [Users] ON;
                    INSERT INTO [Users] ([Id], [Username], [Password], [UserRoleId], [UserProfileId], [LGUId], [DepartmentId], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [MustChangePassword])
                    VALUES (108, 'releasingofficer', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', @ReleasingOfficerRoleId, 108, 1, NULL, 'System', '2025-01-01T00:00:00.0000000Z', NULL, NULL, 0);
                    SET IDENTITY_INSERT [Users] OFF;

                    SET IDENTITY_INSERT [UserProfiles] ON;
                    INSERT INTO [UserProfiles] ([Id], [UserId], [FirstName], [MiddleName], [LastName], [Email], [MobileNo], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
                    VALUES (108, 108, 'Liza', '', 'Mendoza', 'releasingofficer@lgu.gov.ph', '09171000009', 'System', '2025-01-01T00:00:00.0000000Z', NULL, NULL);
                    SET IDENTITY_INSERT [UserProfiles] OFF;
                END;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_IssuedById",
                table: "Applications",
                column: "IssuedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Users_IssuedById",
                table: "Applications",
                column: "IssuedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Users_IssuedById",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_IssuedById",
                table: "Applications");

            migrationBuilder.Sql(
                """
                DELETE FROM [UserProfiles]
                WHERE [Id] = 108 AND [Email] = 'releasingofficer@lgu.gov.ph';

                DELETE FROM [Users]
                WHERE [Id] = 108 AND [Username] = 'releasingofficer';

                DELETE FROM [UserRoles]
                WHERE [Id] = 12 AND [UserRoleDesc] = 'releasing-officer';
                """);

            migrationBuilder.DropColumn(
                name: "IssuedAt",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "IssuedById",
                table: "Applications");

            migrationBuilder.AlterColumn<string>(
                name: "Accessories",
                table: "BuildingPermits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
