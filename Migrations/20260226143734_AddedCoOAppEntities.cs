using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedCoOAppEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoOApps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BldgPermitNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjLocBlock = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjLocLot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjLocStreet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    LGUId = table.Column<int>(type: "int", nullable: false),
                    BarangayId = table.Column<int>(type: "int", nullable: false),
                    OccupancyNatureId = table.Column<int>(type: "int", nullable: false),
                    FloorArea = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    NoOfStoreys = table.Column<int>(type: "int", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicantTypeId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TIN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DigitalSignature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfSignature = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoOApps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoOApps_ApplicantTypes_ApplicantTypeId",
                        column: x => x.ApplicantTypeId,
                        principalTable: "ApplicantTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoOApps_Barangays_BarangayId",
                        column: x => x.BarangayId,
                        principalTable: "Barangays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoOApps_LGUs_LGUId",
                        column: x => x.LGUId,
                        principalTable: "LGUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoOApps_OccupancyNatures_OccupancyNatureId",
                        column: x => x.OccupancyNatureId,
                        principalTable: "OccupancyNatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoOApps_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CoOAppProfs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoOAppId = table.Column<int>(type: "int", nullable: false),
                    IoCFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IoCPRCNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IoCPTRNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IOCValidity = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EoRFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EoRPRCorPTRNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EoRValidity = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EoRSpecialization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoOAppProfs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoOAppProfs_CoOApps_CoOAppId",
                        column: x => x.CoOAppId,
                        principalTable: "CoOApps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoOAppReqDocs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoOAppId = table.Column<int>(type: "int", nullable: false),
                    ReqDocBldgPermitSPlans = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReqDocAsBuiltPlans = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReqDocConsLogbook = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReqDocConsPhotos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReqDocBrgyClearance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReqDocFSIC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReqDocOthers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoOAppReqDocs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoOAppReqDocs_CoOApps_CoOAppId",
                        column: x => x.CoOAppId,
                        principalTable: "CoOApps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_CoOAppProfs_CoOAppId",
                table: "CoOAppProfs",
                column: "CoOAppId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoOAppReqDocs_CoOAppId",
                table: "CoOAppReqDocs",
                column: "CoOAppId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoOApps_ApplicantTypeId",
                table: "CoOApps",
                column: "ApplicantTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CoOApps_BarangayId",
                table: "CoOApps",
                column: "BarangayId");

            migrationBuilder.CreateIndex(
                name: "IX_CoOApps_LGUId",
                table: "CoOApps",
                column: "LGUId");

            migrationBuilder.CreateIndex(
                name: "IX_CoOApps_OccupancyNatureId",
                table: "CoOApps",
                column: "OccupancyNatureId");

            migrationBuilder.CreateIndex(
                name: "IX_CoOApps_ProvinceId",
                table: "CoOApps",
                column: "ProvinceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoOAppProfs");

            migrationBuilder.DropTable(
                name: "CoOAppReqDocs");

            migrationBuilder.DropTable(
                name: "CoOApps");

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
        }
    }
}
