using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedManyRequirementsToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_RequirementCategorys_RequirementCategoryId",
                table: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_Requirements_RequirementCategoryId",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "RequirementCategoryId",
                table: "Requirements");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 17, 24, 15, 56, DateTimeKind.Utc).AddTicks(1025));

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 17, 24, 15, 56, DateTimeKind.Utc).AddTicks(1009));

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 17, 24, 15, 56, DateTimeKind.Utc).AddTicks(990));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 17, 24, 15, 56, DateTimeKind.Utc).AddTicks(852));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 17, 24, 15, 56, DateTimeKind.Utc).AddTicks(854));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 17, 24, 15, 56, DateTimeKind.Utc).AddTicks(855));

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_ReqCatId",
                table: "Requirements",
                column: "ReqCatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_RequirementCategorys_ReqCatId",
                table: "Requirements",
                column: "ReqCatId",
                principalTable: "RequirementCategorys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_RequirementCategorys_ReqCatId",
                table: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_Requirements_ReqCatId",
                table: "Requirements");

            migrationBuilder.AddColumn<int>(
                name: "RequirementCategoryId",
                table: "Requirements",
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
                name: "IX_Requirements_RequirementCategoryId",
                table: "Requirements",
                column: "RequirementCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_RequirementCategorys_RequirementCategoryId",
                table: "Requirements",
                column: "RequirementCategoryId",
                principalTable: "RequirementCategorys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
