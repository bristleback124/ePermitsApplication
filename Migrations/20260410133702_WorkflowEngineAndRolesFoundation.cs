using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class WorkflowEngineAndRolesFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StatusReason",
                table: "Applications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubmittedById",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "UserRoleDesc",
                value: "superadmin");

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "UserRoleDesc" },
                values: new object[,]
                {
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, "encoder" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, "initial-reviewer" },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, "fee-assessor" },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, "final-reviewer" },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, "final-approver" },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, "executive" },
                    { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, "sysadmin" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "MustChangePassword",
                value: false);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_SubmittedById",
                table: "Applications",
                column: "SubmittedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Users_SubmittedById",
                table: "Applications",
                column: "SubmittedById",
                principalTable: "Users",
                principalColumn: "Id");

            // Remap existing application statuses to new workflow values
            migrationBuilder.Sql("UPDATE Applications SET Status = 'Under Initial Review' WHERE Status = 'Under Review';");
            migrationBuilder.Sql("UPDATE Applications SET Status = 'Approved - For Issuance' WHERE Status = 'Approved';");
            migrationBuilder.Sql("UPDATE Applications SET Status = 'Closed - Issued' WHERE Status = 'Closed';");

            // Catch-all: reset any unrecognized status to Submitted
            migrationBuilder.Sql(@"
                UPDATE Applications SET Status = 'Submitted'
                WHERE Status NOT IN (
                    'Draft', 'Submitted', 'Under Initial Review', 'Deficiency Issued',
                    'For Fee Computation', 'Payment Pending', 'For Final Review',
                    'For Final Approval', 'Approved - For Issuance',
                    'Closed - Issued', 'Closed - Rejected', 'Closed - Cancelled'
                );");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Users_SubmittedById",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_SubmittedById",
                table: "Applications");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StatusReason",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "SubmittedById",
                table: "Applications");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "UserRoleDesc",
                value: "admin");

            // Reverse status remapping
            migrationBuilder.Sql("UPDATE Applications SET Status = 'Under Review' WHERE Status = 'Under Initial Review';");
            migrationBuilder.Sql("UPDATE Applications SET Status = 'Approved' WHERE Status = 'Approved - For Issuance';");
            migrationBuilder.Sql("UPDATE Applications SET Status = 'Closed' WHERE Status IN ('Closed - Issued', 'Closed - Rejected', 'Closed - Cancelled');");
        }
    }
}
