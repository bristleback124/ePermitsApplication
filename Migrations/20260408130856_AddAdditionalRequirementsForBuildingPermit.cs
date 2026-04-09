using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalRequirementsForBuildingPermit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TechDocEnvironmentalDocuments",
                table: "BuildingPermitTechDocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechDocFireSafetyPlans",
                table: "BuildingPermitTechDocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechDocSoilTestFieldDensityTest",
                table: "BuildingPermitTechDocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechDocStructuralAnalysisDesign",
                table: "BuildingPermitTechDocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Accessories",
                table: "BuildingPermits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GSEFullName",
                table: "BuildingPermitDesignProfs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GSEPRCNo",
                table: "BuildingPermitDesignProfs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GSEPTRNo",
                table: "BuildingPermitDesignProfs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GSEValidity",
                table: "BuildingPermitDesignProfs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BuildingPermitSupportingDocs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildingPermitId = table.Column<int>(type: "int", nullable: false),
                    SupportDocZoningClearance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportDocLocationalClearance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportDocFireSafetyClearance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportDocHighwayClearance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportDocHeightClearance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportDocECCorCNC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportDocDENRClearance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportDocSECRegistration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportDocBoardResolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportDocHOAClearance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingPermitSupportingDocs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildingPermitSupportingDocs_BuildingPermits_BuildingPermitId",
                        column: x => x.BuildingPermitId,
                        principalTable: "BuildingPermits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermitSupportingDocs_BuildingPermitId",
                table: "BuildingPermitSupportingDocs",
                column: "BuildingPermitId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildingPermitSupportingDocs");

            migrationBuilder.DropColumn(
                name: "TechDocEnvironmentalDocuments",
                table: "BuildingPermitTechDocs");

            migrationBuilder.DropColumn(
                name: "TechDocFireSafetyPlans",
                table: "BuildingPermitTechDocs");

            migrationBuilder.DropColumn(
                name: "TechDocSoilTestFieldDensityTest",
                table: "BuildingPermitTechDocs");

            migrationBuilder.DropColumn(
                name: "TechDocStructuralAnalysisDesign",
                table: "BuildingPermitTechDocs");

            migrationBuilder.DropColumn(
                name: "Accessories",
                table: "BuildingPermits");

            migrationBuilder.DropColumn(
                name: "GSEFullName",
                table: "BuildingPermitDesignProfs");

            migrationBuilder.DropColumn(
                name: "GSEPRCNo",
                table: "BuildingPermitDesignProfs");

            migrationBuilder.DropColumn(
                name: "GSEPTRNo",
                table: "BuildingPermitDesignProfs");

            migrationBuilder.DropColumn(
                name: "GSEValidity",
                table: "BuildingPermitDesignProfs");
        }
    }
}
