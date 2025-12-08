using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorGameAPI.Migrations
{
    /// <inheritdoc />
    public partial class Score : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "GameHistories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "GameHistories");
        }
    }
}
