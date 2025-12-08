using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorGameAPI.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelChangesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaused",
                table: "GameSessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSaved",
                table: "GameSessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "StateJson",
                table: "GameSessions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaused",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "LastSaved",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "StateJson",
                table: "GameSessions");
        }
    }
}
