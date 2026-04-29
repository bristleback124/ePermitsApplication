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

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "UserRoleDesc" },
                values: new object[] { 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, "releasing-officer" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DepartmentId", "LGUId", "MustChangePassword", "Password", "UpdatedAt", "UpdatedBy", "UserProfileId", "UserRoleId", "Username" },
                values: new object[] { 107, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 1, false, "75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=", null, null, 107, 12, "releasingofficer" });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "FirstName", "LastName", "MiddleName", "MobileNo", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[] { 107, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "releasingofficer@lgu.gov.ph", "Liza", "Mendoza", "", "09171000008", null, null, 107 });

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

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 12);

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
