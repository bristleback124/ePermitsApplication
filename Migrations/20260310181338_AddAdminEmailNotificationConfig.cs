using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminEmailNotificationConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminEmailNotificationConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminEmailNotificationConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminEmailNotificationConfigs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminEmailNotificationConfigs_ApplicationType_UserId",
                table: "AdminEmailNotificationConfigs",
                columns: new[] { "ApplicationType", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminEmailNotificationConfigs_UserId",
                table: "AdminEmailNotificationConfigs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminEmailNotificationConfigs");
        }
    }
}
