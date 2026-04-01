using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminMaintenanceManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationTypeScope",
                table: "Requirements",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Both");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Requirements",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationTypeScope",
                table: "RequirementClassifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Both");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RequirementClassifications",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationTypeScope",
                table: "RequirementCategorys",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Both");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RequirementCategorys",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Provinces",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProjectClassifications",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PermitApplicationTypes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "OwnershipTypes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "OccupancyNatures",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LGUs",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Barangays",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ApplicantTypes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "ApplicantTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "ApplicantTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 8,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 9,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 10,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 11,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 12,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 13,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 14,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 15,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 16,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 17,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 18,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 19,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Barangays",
                keyColumn: "Id",
                keyValue: 20,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "LGUs",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "OccupancyNatures",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "OccupancyNatures",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "OccupancyNatures",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "OccupancyNatures",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "OccupancyNatures",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "OccupancyNatures",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "OwnershipTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "OwnershipTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "OwnershipTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "PermitApplicationTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "PermitApplicationTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "PermitApplicationTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "PermitApplicationTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "PermitApplicationTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "PermitApplicationTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "PermitApplicationTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "ProjectClassifications",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "ProjectClassifications",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Provinces",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "RequirementClassifications",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "RequirementClassifications",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });

            migrationBuilder.UpdateData(
                table: "Requirements",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "ApplicationTypeScope", "IsActive" },
                values: new object[] { "Both", true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationTypeScope",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "ApplicationTypeScope",
                table: "RequirementClassifications");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RequirementClassifications");

            migrationBuilder.DropColumn(
                name: "ApplicationTypeScope",
                table: "RequirementCategorys");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RequirementCategorys");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProjectClassifications");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PermitApplicationTypes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "OwnershipTypes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "OccupancyNatures");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LGUs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Barangays");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ApplicantTypes");
        }
    }
}
