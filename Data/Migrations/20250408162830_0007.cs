using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class _0007 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Players_BlackPlayerId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Players_WhitePlayerId",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_BlackPlayerId",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_WhitePlayerId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "BlackConfirmed",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "BlackPlayerId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "WhiteConfirmed",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "WhitePlayerId",
                table: "Matches");

            migrationBuilder.CreateTable(
                name: "PlayerMatches",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    MatchId = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<int>(type: "integer", nullable: false),
                    HasConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    MatchOutcome = table.Column<int>(type: "integer", nullable: false),
                    Outcome = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerMatches", x => new { x.PlayerId, x.MatchId });
                    table.ForeignKey(
                        name: "FK_PlayerMatches_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerMatches_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatches_MatchId",
                table: "PlayerMatches",
                column: "MatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerMatches");

            migrationBuilder.AddColumn<bool>(
                name: "BlackConfirmed",
                table: "Matches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BlackPlayerId",
                table: "Matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "WhiteConfirmed",
                table: "Matches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WhitePlayerId",
                table: "Matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_BlackPlayerId",
                table: "Matches",
                column: "BlackPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_WhitePlayerId",
                table: "Matches",
                column: "WhitePlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Players_BlackPlayerId",
                table: "Matches",
                column: "BlackPlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Players_WhitePlayerId",
                table: "Matches",
                column: "WhitePlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}