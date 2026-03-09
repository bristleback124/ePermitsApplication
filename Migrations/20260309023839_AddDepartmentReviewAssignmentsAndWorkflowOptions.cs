using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentReviewAssignmentsAndWorkflowOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "ApplicationDepartmentReviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignedReviewerId",
                table: "ApplicationDepartmentReviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationDepartmentReviews_AssignedReviewerId",
                table: "ApplicationDepartmentReviews",
                column: "AssignedReviewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationDepartmentReviews_Users_AssignedReviewerId",
                table: "ApplicationDepartmentReviews",
                column: "AssignedReviewerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
UPDATE Applications
SET Status = 'Submitted'
WHERE Status IN ('submitted', 'Pending');
");

            migrationBuilder.Sql(@"
DELETE adr
FROM ApplicationDepartmentReviews adr
INNER JOIN Applications a ON a.Id = adr.ApplicationId
WHERE a.Type = 'CertificateOfOccupancy'
  AND adr.DepartmentId = 2;
");

            migrationBuilder.Sql(@"
INSERT INTO ApplicationDepartmentReviews (ApplicationId, DepartmentId, Status, CreatedAt, UpdatedAt, AssignedReviewerId, AssignedAt)
SELECT a.Id, 3, 'In Queue', SYSUTCDATETIME(), NULL, NULL, NULL
FROM Applications a
WHERE a.Type = 'CertificateOfOccupancy'
  AND NOT EXISTS (
      SELECT 1
      FROM ApplicationDepartmentReviews adr
      WHERE adr.ApplicationId = a.Id
        AND adr.DepartmentId = 3
  );
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationDepartmentReviews_Users_AssignedReviewerId",
                table: "ApplicationDepartmentReviews");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationDepartmentReviews_AssignedReviewerId",
                table: "ApplicationDepartmentReviews");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "ApplicationDepartmentReviews");

            migrationBuilder.DropColumn(
                name: "AssignedReviewerId",
                table: "ApplicationDepartmentReviews");
        }
    }
}
