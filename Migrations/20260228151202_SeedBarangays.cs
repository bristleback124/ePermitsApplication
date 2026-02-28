using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedBarangays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Barangays",
                columns: new[] { "Id", "BarangayName", "CreatedAt", "CreatedBy", "IsDeleted", "LGUId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "Cabangahan", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 2, "Cansaga", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 3, "Casili", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 4, "Danglag", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 5, "Garing", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 6, "Jugan", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 7, "Lamac", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 8, "Lanipga", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 9, "Nangka", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 10, "Panas", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 11, "Pitogo", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 12, "Poblacion East", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 13, "Poblacion Occidental", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 14, "Poblacion Oriental", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 15, "Pulpogan", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 16, "Sacsac", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 17, "Tayud", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 18, "Tilhaong", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 19, "Tolotolo", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null },
                    { 20, "Tugbongan", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", false, 1, null, null }
                });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 28, 15, 12, 2, 24, DateTimeKind.Utc).AddTicks(9288));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 28, 15, 12, 2, 24, DateTimeKind.Utc).AddTicks(9290));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 28, 15, 12, 2, 24, DateTimeKind.Utc).AddTicks(9291));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DepartmentId", "LGUId", "Password", "UpdatedAt", "UpdatedBy", "UserProfileId", "UserRoleId", "Username" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, 1, "75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=", null, null, 1, 1, "admin" });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "FirstName", "LastName", "MiddleName", "MobileNo", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "admin@epermits.com", "System", "Administrator", "", "0000000000", null, null, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

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
    }
}
