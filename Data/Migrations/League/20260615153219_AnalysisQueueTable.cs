using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations.League
{
    /// <inheritdoc />
    public partial class AnalysisQueueTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameAnalysisId",
                table: "Matches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GameAnalysisUrl",
                table: "Matches",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GameAnalysis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Sgf = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameAnalysis", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_GameAnalysisId",
                table: "Matches",
                column: "GameAnalysisId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_GameAnalysis_GameAnalysisId",
                table: "Matches",
                column: "GameAnalysisId",
                principalTable: "GameAnalysis",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_GameAnalysis_GameAnalysisId",
                table: "Matches");

            migrationBuilder.DropTable(
                name: "GameAnalysis");

            migrationBuilder.DropIndex(
                name: "IX_Matches_GameAnalysisId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "GameAnalysisId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "GameAnalysisUrl",
                table: "Matches");
        }
    }
}
