using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Queue
{
    /// <inheritdoc />
    public partial class StateLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "GameAnalysis");

            migrationBuilder.AddColumn<string>(
                name: "StateUrl",
                table: "GameAnalysis",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StateUrl",
                table: "GameAnalysis");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "GameAnalysis",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
