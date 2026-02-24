using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicantRoleAndSeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 9, 30, 27, 663, DateTimeKind.Utc).AddTicks(5097));

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 9, 30, 27, 663, DateTimeKind.Utc).AddTicks(5051));

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 9, 30, 27, 663, DateTimeKind.Utc).AddTicks(5015));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 9, 30, 27, 663, DateTimeKind.Utc).AddTicks(4569));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 9, 30, 27, 663, DateTimeKind.Utc).AddTicks(4702));

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "UserRoleDesc" },
                values: new object[] { 3, new DateTime(2026, 2, 11, 9, 30, 27, 663, DateTimeKind.Utc).AddTicks(4705), "System", null, null, "applicant" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DepartmentId", "LGUId", "Password", "UpdatedAt", "UpdatedBy", "UserProfileId", "UserRoleId", "Username" },
                values: new object[,]
                {
                    { 12, new DateTime(2026, 2, 11, 9, 30, 27, 663, DateTimeKind.Utc).AddTicks(4914), "System", 1, 1, "x5nVHcb0n/OnHgvIDvYRiBbLrp5oD8aLxBCdL6V8mB4=", null, null, null, 1, "admin" },
                    { 13, new DateTime(2026, 2, 11, 9, 30, 27, 663, DateTimeKind.Utc).AddTicks(4919), "System", 1, 1, "x5nVHcb0n/OnHgvIDvYRiBbLrp5oD8aLxBCdL6V8mB4=", null, null, null, 2, "govtuser" }
                });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "FirstName", "LastName", "MiddleName", "MobileNo", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { 10, new DateTime(2026, 2, 11, 9, 30, 27, 663, DateTimeKind.Utc).AddTicks(4963), "System", "admin@epermits.gov", "Admin", "User", "System", "09171234567", null, null, 12 },
                    { 11, new DateTime(2026, 2, 11, 9, 30, 27, 663, DateTimeKind.Utc).AddTicks(4968), "System", "govtuser@epermits.gov", "Government", "User", "IT", "09171234568", null, null, 13 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DepartmentId", "LGUId", "Password", "UpdatedAt", "UpdatedBy", "UserProfileId", "UserRoleId", "Username" },
                values: new object[] { 14, new DateTime(2026, 2, 11, 9, 30, 27, 663, DateTimeKind.Utc).AddTicks(4922), "System", null, 1, "x5nVHcb0n/OnHgvIDvYRiBbLrp5oD8aLxBCdL6V8mB4=", null, null, null, 3, "applicant" });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "FirstName", "LastName", "MiddleName", "MobileNo", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[] { 12, new DateTime(2026, 2, 11, 9, 30, 27, 663, DateTimeKind.Utc).AddTicks(4971), "System", "applicant@example.com", "Juan", "Cruz", "Dela", "09171234569", null, null, 14 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 24, 7, 36, 48, 113, DateTimeKind.Utc).AddTicks(598));

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 24, 7, 36, 48, 113, DateTimeKind.Utc).AddTicks(567));

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 24, 7, 36, 48, 113, DateTimeKind.Utc).AddTicks(538));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 24, 7, 36, 48, 113, DateTimeKind.Utc).AddTicks(390));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 24, 7, 36, 48, 113, DateTimeKind.Utc).AddTicks(392));
        }
    }
}
