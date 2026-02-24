using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeededUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 11, 8, 18, 7, DateTimeKind.Utc).AddTicks(9180));

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 11, 8, 18, 7, DateTimeKind.Utc).AddTicks(9132));

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 11, 8, 18, 7, DateTimeKind.Utc).AddTicks(9081));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 11, 8, 18, 7, DateTimeKind.Utc).AddTicks(8508));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 11, 8, 18, 7, DateTimeKind.Utc).AddTicks(8512));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 11, 8, 18, 7, DateTimeKind.Utc).AddTicks(8514));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 9, 50, 32, 293, DateTimeKind.Utc).AddTicks(4863));

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 9, 50, 32, 293, DateTimeKind.Utc).AddTicks(4820));

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 9, 50, 32, 293, DateTimeKind.Utc).AddTicks(4780));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 9, 50, 32, 293, DateTimeKind.Utc).AddTicks(4416));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 9, 50, 32, 293, DateTimeKind.Utc).AddTicks(4421));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 9, 50, 32, 293, DateTimeKind.Utc).AddTicks(4424));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DepartmentId", "LGUId", "Password", "UpdatedAt", "UpdatedBy", "UserProfileId", "UserRoleId", "Username" },
                values: new object[,]
                {
                    { 12, new DateTime(2026, 2, 11, 9, 50, 32, 293, DateTimeKind.Utc).AddTicks(4639), "System", 1, 1, "Password123!", null, null, 10, 1, "admin" },
                    { 13, new DateTime(2026, 2, 11, 9, 50, 32, 293, DateTimeKind.Utc).AddTicks(4645), "System", 1, 1, "Password123!", null, null, 11, 2, "govtuser" },
                    { 14, new DateTime(2026, 2, 11, 9, 50, 32, 293, DateTimeKind.Utc).AddTicks(4648), "System", null, 1, "Password123!", null, null, 12, 3, "applicant" }
                });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "FirstName", "LastName", "MiddleName", "MobileNo", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { 10, new DateTime(2026, 2, 11, 9, 50, 32, 293, DateTimeKind.Utc).AddTicks(4722), "System", "admin@epermits.gov", "Admin", "User", "System", "09171234567", null, null, 12 },
                    { 11, new DateTime(2026, 2, 11, 9, 50, 32, 293, DateTimeKind.Utc).AddTicks(4728), "System", "govtuser@epermits.gov", "Government", "User", "IT", "09171234568", null, null, 13 },
                    { 12, new DateTime(2026, 2, 11, 9, 50, 32, 293, DateTimeKind.Utc).AddTicks(4732), "System", "applicant@example.com", "Juan", "Cruz", "Dela", "09171234569", null, null, 14 }
                });
        }
    }
}
