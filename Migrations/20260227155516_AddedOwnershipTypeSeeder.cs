using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedOwnershipTypeSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 55, 15, 638, DateTimeKind.Utc).AddTicks(90));

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 55, 15, 638, DateTimeKind.Utc).AddTicks(70));

            migrationBuilder.InsertData(
                table: "OwnershipTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "OwnershipTypeDesc", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, "Owner", null, null },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, "Lessee", null, null },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, "Authorized Representative", null, null }
                });

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 55, 15, 638, DateTimeKind.Utc).AddTicks(45));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 55, 15, 637, DateTimeKind.Utc).AddTicks(9872));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 55, 15, 637, DateTimeKind.Utc).AddTicks(9874));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 15, 55, 15, 637, DateTimeKind.Utc).AddTicks(9875));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OwnershipTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OwnershipTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OwnershipTypes",
                keyColumn: "Id",
                keyValue: 3);

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
    }
}
