using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorGameAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemovingPlayerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "Users",
                type: "integer",
                nullable: true);
        }
    }
}
