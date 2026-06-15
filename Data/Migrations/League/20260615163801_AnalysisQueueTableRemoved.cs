using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.League
{
    /// <inheritdoc />
    public partial class AnalysisQueueTableRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_GameAnalysis_GameAnalysisId",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_GameAnalysisId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "GameAnalysisId",
                table: "Matches");

            migrationBuilder.AddColumn<int>(
                name: "GameAnalysisStatus",
                table: "Matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MatchId",
                table: "GameAnalysis",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameAnalysisStatus",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "GameAnalysis");

            migrationBuilder.AddColumn<int>(
                name: "GameAnalysisId",
                table: "Matches",
                type: "integer",
                nullable: true);

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
    }
}
