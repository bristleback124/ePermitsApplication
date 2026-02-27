using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationIdToCoOApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "CoOApps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 14, 56, 59, 84, DateTimeKind.Utc).AddTicks(7026));

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 14, 56, 59, 84, DateTimeKind.Utc).AddTicks(6999));

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 14, 56, 59, 84, DateTimeKind.Utc).AddTicks(6968));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 14, 56, 59, 84, DateTimeKind.Utc).AddTicks(6742));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 14, 56, 59, 84, DateTimeKind.Utc).AddTicks(6745));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 14, 56, 59, 84, DateTimeKind.Utc).AddTicks(6746));

            migrationBuilder.CreateIndex(
                name: "IX_CoOApps_ApplicationId",
                table: "CoOApps",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CoOApps_Applications_ApplicationId",
                table: "CoOApps",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoOApps_Applications_ApplicationId",
                table: "CoOApps");

            migrationBuilder.DropIndex(
                name: "IX_CoOApps_ApplicationId",
                table: "CoOApps");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "CoOApps");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 14, 37, 33, 918, DateTimeKind.Utc).AddTicks(5876));

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 14, 37, 33, 918, DateTimeKind.Utc).AddTicks(5857));

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 14, 37, 33, 918, DateTimeKind.Utc).AddTicks(5838));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 14, 37, 33, 918, DateTimeKind.Utc).AddTicks(5677));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 14, 37, 33, 918, DateTimeKind.Utc).AddTicks(5681));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 14, 37, 33, 918, DateTimeKind.Utc).AddTicks(5682));
        }
    }
}
