using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedProjectClassificationSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 50, 49, 826, DateTimeKind.Utc).AddTicks(2738));

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 50, 49, 826, DateTimeKind.Utc).AddTicks(2719));

            migrationBuilder.InsertData(
                table: "ProjectClassifications",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "ProjectClassDesc", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, "Private", null, null },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, "Government", null, null }
                });

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 50, 49, 826, DateTimeKind.Utc).AddTicks(2695));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 50, 49, 826, DateTimeKind.Utc).AddTicks(2527));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 50, 49, 826, DateTimeKind.Utc).AddTicks(2531));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 50, 49, 826, DateTimeKind.Utc).AddTicks(2532));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProjectClassifications",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProjectClassifications",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 45, 53, 512, DateTimeKind.Utc).AddTicks(4434));

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 45, 53, 512, DateTimeKind.Utc).AddTicks(4413));

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 45, 53, 512, DateTimeKind.Utc).AddTicks(4391));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 45, 53, 512, DateTimeKind.Utc).AddTicks(4227));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 45, 53, 512, DateTimeKind.Utc).AddTicks(4231));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 45, 53, 512, DateTimeKind.Utc).AddTicks(4232));
        }
    }
}
