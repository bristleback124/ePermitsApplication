using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddBuildingPermitEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuildingPermits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermitAppTypeId = table.Column<int>(type: "int", nullable: false),
                    OccupancyNatureId = table.Column<int>(type: "int", nullable: false),
                    ProjectTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectClassId = table.Column<int>(type: "int", nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    NoOfStoreys = table.Column<int>(type: "int", nullable: false),
                    FloorAreaPerStorey = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    TotalFloorArea = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    ProjectScopeLotArea = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    PropertyAddBlock = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropertyAddLot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyAddStreet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    LGUId = table.Column<int>(type: "int", nullable: false),
                    BarangayId = table.Column<int>(type: "int", nullable: false),
                    PropertyDetailLotArea = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    TCTNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaxDeclarionNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Coordinates = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DigitalSignature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateofSignature = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingPermits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildingPermits_Barangays_BarangayId",
                        column: x => x.BarangayId,
                        principalTable: "Barangays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuildingPermits_LGUs_LGUId",
                        column: x => x.LGUId,
                        principalTable: "LGUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuildingPermits_OccupancyNatures_OccupancyNatureId",
                        column: x => x.OccupancyNatureId,
                        principalTable: "OccupancyNatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuildingPermits_PermitApplicationTypes_PermitAppTypeId",
                        column: x => x.PermitAppTypeId,
                        principalTable: "PermitApplicationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuildingPermits_ProjectClassifications_ProjectClassId",
                        column: x => x.ProjectClassId,
                        principalTable: "ProjectClassifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuildingPermits_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuildingPermitAppInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildingPermitId = table.Column<int>(type: "int", nullable: false),
                    ApplicantTypeId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TIN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnershipTypeId = table.Column<int>(type: "int", nullable: false),
                    ReqDocProofOwnership = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReqDocBarangayClearance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReqDocTaxDeclaration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReqDocRealPropTaxReceipt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReqDocECCorCNC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReqDocSpecialClearances = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingPermitAppInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildingPermitAppInfos_ApplicantTypes_ApplicantTypeId",
                        column: x => x.ApplicantTypeId,
                        principalTable: "ApplicantTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuildingPermitAppInfos_BuildingPermits_BuildingPermitId",
                        column: x => x.BuildingPermitId,
                        principalTable: "BuildingPermits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildingPermitAppInfos_OwnershipTypes_OwnershipTypeId",
                        column: x => x.OwnershipTypeId,
                        principalTable: "OwnershipTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuildingPermitDesignProfs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildingPermitId = table.Column<int>(type: "int", nullable: false),
                    IoCFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IoCPRCNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IoCPTRNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IOCValidity = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SEFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SEPRCNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SEPTRNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SEValidity = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EEFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EEPRCNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EEPTRNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EEValidity = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SPEFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SPEPRCNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SPEPTRNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SPEValidity = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MEFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MEPRCNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MEPTRNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MEValidity = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ECEFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ECEPRCNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ECEPTRNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ECEValidity = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractorBusinessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractorPCABNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContractorClassification = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContractorValidity = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingPermitDesignProfs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildingPermitDesignProfs_BuildingPermits_BuildingPermitId",
                        column: x => x.BuildingPermitId,
                        principalTable: "BuildingPermits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildingPermitTechDocs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildingPermitId = table.Column<int>(type: "int", nullable: false),
                    TechDocIoCPlans = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechDocSEPlans = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechDocEEPlans = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechDocSPPlans = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechDocBOMCost = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechDocSoW = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechDocMEPlans = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TechDocECEPlans = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingPermitTechDocs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildingPermitTechDocs_BuildingPermits_BuildingPermitId",
                        column: x => x.BuildingPermitId,
                        principalTable: "BuildingPermits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermitAppInfos_ApplicantTypeId",
                table: "BuildingPermitAppInfos",
                column: "ApplicantTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermitAppInfos_BuildingPermitId",
                table: "BuildingPermitAppInfos",
                column: "BuildingPermitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermitAppInfos_OwnershipTypeId",
                table: "BuildingPermitAppInfos",
                column: "OwnershipTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermitDesignProfs_BuildingPermitId",
                table: "BuildingPermitDesignProfs",
                column: "BuildingPermitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermits_BarangayId",
                table: "BuildingPermits",
                column: "BarangayId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermits_LGUId",
                table: "BuildingPermits",
                column: "LGUId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermits_OccupancyNatureId",
                table: "BuildingPermits",
                column: "OccupancyNatureId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermits_PermitAppTypeId",
                table: "BuildingPermits",
                column: "PermitAppTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermits_ProjectClassId",
                table: "BuildingPermits",
                column: "ProjectClassId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermits_ProvinceId",
                table: "BuildingPermits",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPermitTechDocs_BuildingPermitId",
                table: "BuildingPermitTechDocs",
                column: "BuildingPermitId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildingPermitAppInfos");

            migrationBuilder.DropTable(
                name: "BuildingPermitDesignProfs");

            migrationBuilder.DropTable(
                name: "BuildingPermitTechDocs");

            migrationBuilder.DropTable(
                name: "BuildingPermits");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 11, 8, 18, 7, DateTimeKind.Utc).AddTicks(9180));

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 11, 8, 18, 7, DateTimeKind.Utc).AddTicks(9132));

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 11, 8, 18, 7, DateTimeKind.Utc).AddTicks(9081));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 11, 8, 18, 7, DateTimeKind.Utc).AddTicks(8508));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 11, 8, 18, 7, DateTimeKind.Utc).AddTicks(8512));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 11, 11, 8, 18, 7, DateTimeKind.Utc).AddTicks(8514));
        }
    }
}
