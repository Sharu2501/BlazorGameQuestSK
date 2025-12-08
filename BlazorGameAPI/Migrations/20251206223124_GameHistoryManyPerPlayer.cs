using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorGameAPI.Migrations
{
    /// <inheritdoc />
    public partial class GameHistoryManyPerPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameHistories_PlayerId",
                table: "GameHistories");

            migrationBuilder.CreateIndex(
                name: "IX_GameHistories_PlayerId",
                table: "GameHistories",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameHistories_PlayerId",
                table: "GameHistories");

            migrationBuilder.CreateIndex(
                name: "IX_GameHistories_PlayerId",
                table: "GameHistories",
                column: "PlayerId",
                unique: true);
        }
    }
}
