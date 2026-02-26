using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationIdToBuildinPermit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "BuildingPermits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 4, 11, 43, 715, DateTimeKind.Utc).AddTicks(364));

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 4, 11, 43, 715, DateTimeKind.Utc).AddTicks(339));

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 4, 11, 43, 715, DateTimeKind.Utc).AddTicks(314));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 4, 11, 43, 715, DateTimeKind.Utc).AddTicks(120));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 4, 11, 43, 715, DateTimeKind.Utc).AddTicks(122));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 4, 11, 43, 715, DateTimeKind.Utc).AddTicks(124));

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermits_ApplicationId",
                table: "BuildingPermits",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BuildingPermits_Applications_ApplicationId",
                table: "BuildingPermits",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuildingPermits_Applications_ApplicationId",
                table: "BuildingPermits");

            migrationBuilder.DropIndex(
                name: "IX_BuildingPermits_ApplicationId",
                table: "BuildingPermits");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "BuildingPermits");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 3, 3, 20, 203, DateTimeKind.Utc).AddTicks(1104));

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 3, 3, 20, 203, DateTimeKind.Utc).AddTicks(1072));

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 3, 3, 20, 203, DateTimeKind.Utc).AddTicks(1036));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 3, 3, 20, 203, DateTimeKind.Utc).AddTicks(684));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 3, 3, 20, 203, DateTimeKind.Utc).AddTicks(687));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 3, 3, 20, 203, DateTimeKind.Utc).AddTicks(690));
        }
    }
}
