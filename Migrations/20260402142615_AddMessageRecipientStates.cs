using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePermitsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageRecipientStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageRecipientStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    RecipientUserId = table.Column<int>(type: "int", nullable: false),
                    RecipientRole = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SenderType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageRecipientStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageRecipientStates_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageRecipientStates_Users_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipientStates_MessageId_RecipientUserId",
                table: "MessageRecipientStates",
                columns: new[] { "MessageId", "RecipientUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipientStates_RecipientUserId_IsRead_SenderType",
                table: "MessageRecipientStates",
                columns: new[] { "RecipientUserId", "IsRead", "SenderType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageRecipientStates");
        }
    }
}
