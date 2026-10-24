using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PerformedByRole",
                table: "AuditTrails",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserRoleAssignments",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserRoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleAssignments", x => new { x.UserId, x.UserRoleId });
                    table.ForeignKey(
                        name: "FK_UserRoleAssignments_UserRoles_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoleAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_UserRoleId",
                table: "UserRoleAssignments",
                column: "UserRoleId");

            // Every existing user keeps exactly their current role. Seeded users are covered here too,
            // so the join table has no HasData (it would duplicate-key against this backfill).
            migrationBuilder.Sql(
                "INSERT INTO [UserRoleAssignments] ([UserId], [UserRoleId]) SELECT [Id], [UserRoleId] FROM [Users];");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRoleAssignments");

            migrationBuilder.DropColumn(
                name: "PerformedByRole",
                table: "AuditTrails");
        }
    }
}
