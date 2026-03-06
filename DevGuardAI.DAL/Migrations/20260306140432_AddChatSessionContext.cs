using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevGuardAI.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddChatSessionContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContextSummary",
                table: "ChatSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SessionType",
                table: "ChatSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContextSummary",
                table: "ChatSessions");

            migrationBuilder.DropColumn(
                name: "SessionType",
                table: "ChatSessions");
        }
    }
}
