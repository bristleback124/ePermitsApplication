using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesAndPaginationSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LGUs_ProvinceId",
                table: "LGUs");

            migrationBuilder.AlterColumn<string>(
                name: "ProvinceName",
                table: "Provinces",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LGUName",
                table: "LGUs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_ProvinceName",
                table: "Provinces",
                column: "ProvinceName");

            migrationBuilder.CreateIndex(
                name: "IX_LGUs_ProvinceId_LGUName",
                table: "LGUs",
                columns: new[] { "ProvinceId", "LGUName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Provinces_ProvinceName",
                table: "Provinces");

            migrationBuilder.DropIndex(
                name: "IX_LGUs_ProvinceId_LGUName",
                table: "LGUs");

            migrationBuilder.AlterColumn<string>(
                name: "ProvinceName",
                table: "Provinces",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LGUName",
                table: "LGUs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_LGUs_ProvinceId",
                table: "LGUs",
                column: "ProvinceId");
        }
    }
}
