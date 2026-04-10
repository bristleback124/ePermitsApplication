using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedWorkflowRoleUsersData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DepartmentId", "LGUId", "MustChangePassword", "Password", "UpdatedAt", "UpdatedBy", "UserProfileId", "UserRoleId", "Username" },
                values: new object[,]
                {
                    { 100, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 1, false, "75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=", null, null, 100, 4, "encoder" },
                    { 101, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 1, false, "75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=", null, null, 101, 5, "reviewer" },
                    { 102, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 1, false, "75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=", null, null, 102, 6, "assessor" },
                    { 103, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 1, false, "75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=", null, null, 103, 7, "finalreviewer" },
                    { 104, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 1, false, "75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=", null, null, 104, 8, "approver" },
                    { 105, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 1, false, "75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=", null, null, 105, 9, "mayor" },
                    { 106, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 1, false, "75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=", null, null, 106, 10, "sysadmin" }
                });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "FirstName", "LastName", "MiddleName", "MobileNo", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { 100, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "encoder@lgu.gov.ph", "Juan", "Dela Cruz", "", "09171000001", null, null, 100 },
                    { 101, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "reviewer@lgu.gov.ph", "Maria", "Santos", "", "09171000002", null, null, 101 },
                    { 102, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "assessor@lgu.gov.ph", "Pedro", "Cruz", "", "09171000003", null, null, 102 },
                    { 103, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "finalreviewer@lgu.gov.ph", "Ana", "Reyes", "", "09171000004", null, null, 103 },
                    { 104, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "approver@lgu.gov.ph", "Roberto", "Garcia", "", "09171000005", null, null, 104 },
                    { 105, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "mayor@lgu.gov.ph", "Mayor", "Torres", "", "09171000006", null, null, 105 },
                    { 106, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "sysadmin@lgu.gov.ph", "Carl", "Rivera", "", "09171000007", null, null, 106 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 106);
        }
    }
}
