using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDepartmentLGUDependency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_LGUs_LGUId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_LGUId_DepartmentCode",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "LGUId",
                table: "Departments");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentCode",
                table: "Departments",
                column: "DepartmentCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Departments_DepartmentCode",
                table: "Departments");

            migrationBuilder.AddColumn<int>(
                name: "LGUId",
                table: "Departments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "LGUId",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_LGUId_DepartmentCode",
                table: "Departments",
                columns: new[] { "LGUId", "DepartmentCode" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_LGUs_LGUId",
                table: "Departments",
                column: "LGUId",
                principalTable: "LGUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
