using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddBuildingPermitCategoryScopedRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuildingPermitCategoryId",
                table: "Requirements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BuildingPermitCategoryId",
                table: "BuildingPermits",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "BuildingPermitCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingPermitCategories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BuildingPermitCategories",
                columns: new[] { "Id", "CategoryName", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsDeleted", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "Simple", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Basic residential or small-scale projects", true, false, null, null },
                    { 2, "Complex", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Mid-sized or multi-level construction", true, false, null, null },
                    { 3, "Highly Technical", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Large-scale or specialized structures", true, false, null, null }
                });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApplicationTypeScope", "ReqCatDesc" },
                values: new object[] { "BuildingPermit", "A. Application & Clearances" });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApplicationTypeScope", "ReqCatDesc" },
                values: new object[] { "BuildingPermit", "B. Property & Ownership Documents" });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ApplicationTypeScope", "ReqCatDesc" },
                values: new object[] { "BuildingPermit", "C. Survey & Plans" });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ApplicationTypeScope", "ReqCatDesc", "ReqClassId" },
                values: new object[] { "BuildingPermit", "D. Technical Documents", 1 });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ApplicationTypeScope", "ReqCatDesc", "ReqClassId" },
                values: new object[] { "BuildingPermit", "E. Proof of Payment", 1 });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ApplicationTypeScope", "ReqCatDesc", "ReqClassId" },
                values: new object[] { "BuildingPermit", "F. Additional Technical & Regulatory Requirements", 1 });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ApplicationTypeScope", "ReqCatDesc", "ReqClassId" },
                values: new object[] { "BuildingPermit", "G. Specialized Technical Requirements", 1 });

            migrationBuilder.InsertData(
                table: "RequirementCategorys",
                columns: new[] { "Id", "ApplicationTypeScope", "CreatedAt", "CreatedBy", "IsActive", "IsDeleted", "ReqCatDesc", "ReqClassId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 8, "CertificateOfOccupancy", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, "Completion Documents", 2, null, null },
                    { 9, "CertificateOfOccupancy", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, "Inspection Certificates", 2, null, null },
                    { 10, "CertificateOfOccupancy", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, "Compliance Documents", 2, null, null },
                    { 11, "CertificateOfOccupancy", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, "Additional Requirements", 2, null, null }
                });

            migrationBuilder.UpdateData(
                table: "RequirementClassifications",
                keyColumn: "Id",
                keyValue: 1,
                column: "ApplicationTypeScope",
                value: "BuildingPermit");

            migrationBuilder.UpdateData(
                table: "RequirementClassifications",
                keyColumn: "Id",
                keyValue: 2,
                column: "ApplicationTypeScope",
                value: "CertificateOfOccupancy");

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, "Building/Fencing Permit Application Form" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, "Zoning Certification" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, "Locational Clearance" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, "Fire Safety Clearance" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqCatId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, 1, "Barangay Construction Clearance" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, "Tax Declaration (certified true copy)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, "Tax Clearance / Tax Receipt" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, "Certificate of Title (OCT/TCT)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, "Affidavit of Consent (if applicable)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, "Special Power of Attorney (if applicable)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqCatId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, 2, "Residence certificate" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, "Cadastral Survey Plan" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, "Complete Building Plans (signed & sealed)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqCatId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, 4, "Bill of Materials & Cost Estimates" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, "General Specifications" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqCatId", "ReqDesc" },
                values: new object[] { "BuildingPermit", null, 5, "Official Receipts (fees)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqCatId", "ReqDesc" },
                values: new object[] { "BuildingPermit", 2, 6, "Highway Clearance" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqCatId", "ReqDesc" },
                values: new object[] { "BuildingPermit", 2, 6, "Structural Analysis & Design" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqCatId", "ReqDesc" },
                values: new object[] { "BuildingPermit", 2, 6, "Height Clearance" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqCatId", "ReqDesc" },
                values: new object[] { "BuildingPermit", 2, 6, "Certificate of Safety Evaluation" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqCatId", "ReqDesc" },
                values: new object[] { "BuildingPermit", 2, 6, "ECC or CNC" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqCatId", "ReqDesc" },
                values: new object[] { "BuildingPermit", 2, 6, "DENR Clearance" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqCatId", "ReqDesc" },
                values: new object[] { "BuildingPermit", 2, 6, "SEC Registration (if corporation)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", 2, "Board Resolution / Secretary Certificate" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", 2, "Subdivision Approval" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", 2, "Homeowners Association Clearance" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqDesc" },
                values: new object[] { "BuildingPermit", 3, "Highway Clearance" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqCatId", "ReqDesc" },
                values: new object[] { "BuildingPermit", 3, 6, "Structural Analysis & Design" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqCatId", "ReqDesc" },
                values: new object[] { "BuildingPermit", 3, 6, "Height Clearance" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "ApplicationTypeScope", "BuildingPermitCategoryId", "ReqCatId", "ReqDesc" },
                values: new object[] { "BuildingPermit", 3, 6, "Certificate of Safety Evaluation" });

            migrationBuilder.InsertData(
                table: "Requirements",
                columns: new[] { "Id", "ApplicationTypeScope", "BuildingPermitCategoryId", "CreatedAt", "CreatedBy", "IsActive", "IsDeleted", "ReqCatId", "ReqDesc", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 31, "BuildingPermit", 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 6, "ECC or CNC", null, null },
                    { 32, "BuildingPermit", 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 6, "DENR Clearance", null, null },
                    { 33, "BuildingPermit", 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 6, "SEC Registration (if corporation)", null, null },
                    { 34, "BuildingPermit", 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 6, "Board Resolution / Secretary Certificate", null, null },
                    { 35, "BuildingPermit", 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 6, "Subdivision Approval", null, null },
                    { 36, "BuildingPermit", 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 7, "Field Density Test / Soil Test", null, null },
                    { 37, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 8, "Copy of approved Building Permit", null, null },
                    { 38, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 8, "As-Built Plans (signed & sealed by professionals)", null, null },
                    { 39, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 8, "Construction completion certificate", null, null },
                    { 40, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 8, "Material test certificates (concrete, steel, etc.)", null, null },
                    { 41, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 9, "Fire Safety Inspection Certificate (FSIC)", null, null },
                    { 42, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 9, "Electrical Safety Inspection Certificate", null, null },
                    { 43, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 9, "Structural inspection report", null, null },
                    { 44, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 9, "Plumbing inspection certificate", null, null },
                    { 45, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 9, "Mechanical systems inspection (if applicable)", null, null },
                    { 46, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 10, "Environmental compliance monitoring report (if required)", null, null },
                    { 47, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 10, "Accessibility compliance certificate (for public buildings)", null, null },
                    { 48, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 10, "Waste management compliance certificate", null, null },
                    { 49, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 10, "Building maintenance plan", null, null },
                    { 50, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 11, "Updated tax declaration reflecting improvements", null, null },
                    { 51, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 11, "Insurance coverage for the structure", null, null },
                    { 52, "CertificateOfOccupancy", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 11, "Business permits (for commercial/industrial buildings)", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_BuildingPermitCategoryId",
                table: "Requirements",
                column: "BuildingPermitCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermits_BuildingPermitCategoryId",
                table: "BuildingPermits",
                column: "BuildingPermitCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermitCategories_CategoryName",
                table: "BuildingPermitCategories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BuildingPermits_BuildingPermitCategories_BuildingPermitCategoryId",
                table: "BuildingPermits",
                column: "BuildingPermitCategoryId",
                principalTable: "BuildingPermitCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_BuildingPermitCategories_BuildingPermitCategoryId",
                table: "Requirements",
                column: "BuildingPermitCategoryId",
                principalTable: "BuildingPermitCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuildingPermits_BuildingPermitCategories_BuildingPermitCategoryId",
                table: "BuildingPermits");

            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_BuildingPermitCategories_BuildingPermitCategoryId",
                table: "Requirements");

            migrationBuilder.DropTable(
                name: "BuildingPermitCategories");

            migrationBuilder.DropIndex(
                name: "IX_Requirements_BuildingPermitCategoryId",
                table: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_BuildingPermits_BuildingPermitCategoryId",
                table: "BuildingPermits");

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DropColumn(
                name: "BuildingPermitCategoryId",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "BuildingPermitCategoryId",
                table: "BuildingPermits");

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApplicationTypeScope", "ReqCatDesc" },
                values: new object[] { "Both", "Basic Documents" });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApplicationTypeScope", "ReqCatDesc" },
                values: new object[] { "Both", "Technical Plans & Documents" });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ApplicationTypeScope", "ReqCatDesc" },
                values: new object[] { "Both", "Special Requirements (if applicable)" });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ApplicationTypeScope", "ReqCatDesc", "ReqClassId" },
                values: new object[] { "Both", "Completion Documents", 2 });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ApplicationTypeScope", "ReqCatDesc", "ReqClassId" },
                values: new object[] { "Both", "Inspection Certificates", 2 });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ApplicationTypeScope", "ReqCatDesc", "ReqClassId" },
                values: new object[] { "Both", "Compliance Documents", 2 });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ApplicationTypeScope", "ReqCatDesc", "ReqClassId" },
                values: new object[] { "Both", "Additional Requirements", 2 });

            migrationBuilder.UpdateData(
                table: "RequirementClassifications",
                keyColumn: "Id",
                keyValue: 1,
                column: "ApplicationTypeScope",
                value: "Both");

            migrationBuilder.UpdateData(
                table: "RequirementClassifications",
                keyColumn: "Id",
                keyValue: 2,
                column: "ApplicationTypeScope",
                value: "Both");

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Barangay Clearance" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Tax Declaration of Real Property" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Latest Real Property Tax Receipt" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Proof of ownership or right to build (TCT, deed of sale, lease contract)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ApplicationTypeScope", "ReqCatId", "ReqDesc" },
                values: new object[] { "Both", 2, "Architectural Plans (signed & sealed by Licensed Architect)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Structural Plans (signed & sealed by Licensed Civil/Structural Engineer)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Electrical Plans (signed & sealed by Licensed Electrical Engineer)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Plumbing Plans (signed & sealed by Licensed Master Plumber)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Mechanical Plans (if applicable, signed & sealed by Licensed Mechanical Engineer)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Bill of Materials/Specifications" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ApplicationTypeScope", "ReqCatId", "ReqDesc" },
                values: new object[] { "Both", 3, "DPWH clearance (for structures near national roads)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "CAAP clearance (for high-rise buildings)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Environmental Compliance Certificate (ECC)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "ApplicationTypeScope", "ReqCatId", "ReqDesc" },
                values: new object[] { "Both", 3, "Locational clearance from concerned agencies" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Copy of approved Building Permit" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "ApplicationTypeScope", "ReqCatId", "ReqDesc" },
                values: new object[] { "Both", 4, "As-Built Plans (signed & sealed by professionals)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "ApplicationTypeScope", "ReqCatId", "ReqDesc" },
                values: new object[] { "Both", 4, "Construction completion certificate" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "ApplicationTypeScope", "ReqCatId", "ReqDesc" },
                values: new object[] { "Both", 4, "Material test certificates (concrete, steel, etc.)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "ApplicationTypeScope", "ReqCatId", "ReqDesc" },
                values: new object[] { "Both", 5, "Fire Safety Inspection Certificate (FSIC)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "ApplicationTypeScope", "ReqCatId", "ReqDesc" },
                values: new object[] { "Both", 5, "Electrical Safety Inspection Certificate" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "ApplicationTypeScope", "ReqCatId", "ReqDesc" },
                values: new object[] { "Both", 5, "Structural inspection report" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "ApplicationTypeScope", "ReqCatId", "ReqDesc" },
                values: new object[] { "Both", 5, "Plumbing inspection certificate" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "ApplicationTypeScope", "ReqCatId", "ReqDesc" },
                values: new object[] { "Both", 5, "Mechanical systems inspection (if applicable)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Environmental compliance monitoring report (if required)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Accessibility compliance certificate (for public buildings)" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Waste management compliance certificate" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "ApplicationTypeScope", "ReqDesc" },
                values: new object[] { "Both", "Building maintenance plan" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "ApplicationTypeScope", "ReqCatId", "ReqDesc" },
                values: new object[] { "Both", 7, "Updated tax declaration reflecting improvements" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "ApplicationTypeScope", "ReqCatId", "ReqDesc" },
                values: new object[] { "Both", 7, "Insurance coverage for the structure" });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "ApplicationTypeScope", "ReqCatId", "ReqDesc" },
                values: new object[] { "Both", 7, "Business permits (for commercial/industrial buildings)" });
        }
    }
}
