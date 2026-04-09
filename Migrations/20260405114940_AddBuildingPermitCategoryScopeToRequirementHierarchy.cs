using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddBuildingPermitCategoryScopeToRequirementHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuildingPermitCategoryId",
                table: "RequirementClassifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BuildingPermitCategoryId",
                table: "RequirementCategorys",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 1,
                column: "BuildingPermitCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 2,
                column: "BuildingPermitCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 3,
                column: "BuildingPermitCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 4,
                column: "BuildingPermitCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 5,
                column: "BuildingPermitCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 6,
                column: "BuildingPermitCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 7,
                column: "BuildingPermitCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 8,
                column: "BuildingPermitCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 9,
                column: "BuildingPermitCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 10,
                column: "BuildingPermitCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequirementCategorys",
                keyColumn: "Id",
                keyValue: 11,
                column: "BuildingPermitCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequirementClassifications",
                keyColumn: "Id",
                keyValue: 1,
                column: "BuildingPermitCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequirementClassifications",
                keyColumn: "Id",
                keyValue: 2,
                column: "BuildingPermitCategoryId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_RequirementClassifications_BuildingPermitCategoryId",
                table: "RequirementClassifications",
                column: "BuildingPermitCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementCategorys_BuildingPermitCategoryId",
                table: "RequirementCategorys",
                column: "BuildingPermitCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementCategorys_BuildingPermitCategories_BuildingPermitCategoryId",
                table: "RequirementCategorys",
                column: "BuildingPermitCategoryId",
                principalTable: "BuildingPermitCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementClassifications_BuildingPermitCategories_BuildingPermitCategoryId",
                table: "RequirementClassifications",
                column: "BuildingPermitCategoryId",
                principalTable: "BuildingPermitCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequirementCategorys_BuildingPermitCategories_BuildingPermitCategoryId",
                table: "RequirementCategorys");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementClassifications_BuildingPermitCategories_BuildingPermitCategoryId",
                table: "RequirementClassifications");

            migrationBuilder.DropIndex(
                name: "IX_RequirementClassifications_BuildingPermitCategoryId",
                table: "RequirementClassifications");

            migrationBuilder.DropIndex(
                name: "IX_RequirementCategorys_BuildingPermitCategoryId",
                table: "RequirementCategorys");

            migrationBuilder.DropColumn(
                name: "BuildingPermitCategoryId",
                table: "RequirementClassifications");

            migrationBuilder.DropColumn(
                name: "BuildingPermitCategoryId",
                table: "RequirementCategorys");
        }
    }
}
