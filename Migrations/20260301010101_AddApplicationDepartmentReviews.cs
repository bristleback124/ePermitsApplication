using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    [DbContext(typeof(ePermitsApp.Data.ApplicationDbContext))]
    [Migration("20260301010101_AddApplicationDepartmentReviews")]
    public partial class AddApplicationDepartmentReviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationDepartmentReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationDepartmentReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationDepartmentReviews_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationDepartmentReviews_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationDepartmentReviews_ApplicationId_DepartmentId",
                table: "ApplicationDepartmentReviews",
                columns: new[] { "ApplicationId", "DepartmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationDepartmentReviews_DepartmentId",
                table: "ApplicationDepartmentReviews",
                column: "DepartmentId");

            migrationBuilder.Sql(@"
UPDATE Applications
SET Status = 'submitted'
WHERE Status = 'Pending';
");

            migrationBuilder.Sql(@"
INSERT INTO ApplicationDepartmentReviews (ApplicationId, DepartmentId, Status, CreatedAt, UpdatedAt)
SELECT a.Id, departmentIds.DepartmentId, 'In Queue', SYSUTCDATETIME(), NULL
FROM Applications a
CROSS APPLY (
    SELECT 1 AS DepartmentId WHERE a.Type = 'BuildingPermit'
    UNION ALL
    SELECT 2 AS DepartmentId WHERE a.Type IN ('BuildingPermit', 'CertificateOfOccupancy')
    UNION ALL
    SELECT 3 AS DepartmentId WHERE a.Type = 'BuildingPermit'
) departmentIds
WHERE NOT EXISTS (
    SELECT 1
    FROM ApplicationDepartmentReviews adr
    WHERE adr.ApplicationId = a.Id
      AND adr.DepartmentId = departmentIds.DepartmentId
);
");

            migrationBuilder.Sql(@"
INSERT INTO ApplicationDepartmentReviews (ApplicationId, DepartmentId, Status, CreatedAt, UpdatedAt)
SELECT a.Id, 1, 'In Queue', SYSUTCDATETIME(), NULL
FROM Applications a
WHERE a.Type = 'CertificateOfOccupancy'
  AND NOT EXISTS (
      SELECT 1
      FROM ApplicationDepartmentReviews adr
      WHERE adr.ApplicationId = a.Id
        AND adr.DepartmentId = 1
  );
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationDepartmentReviews");
        }
    }
}
