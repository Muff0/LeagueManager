using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class _0003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LeagoKey",
                table: "Players",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BlackPlayerId = table.Column<int>(type: "integer", nullable: false),
                    WhitePlayerId = table.Column<int>(type: "integer", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false),
                    IsComplete = table.Column<bool>(type: "boolean", nullable: false),
                    Round = table.Column<int>(type: "integer", nullable: false),
                    SeasonId = table.Column<int>(type: "integer", nullable: false),
                    PlayerSeasonPlayerId = table.Column<int>(type: "integer", nullable: true),
                    PlayerSeasonSeasonId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_PlayerSeasons_PlayerSeasonPlayerId_PlayerSeasonSeas~",
                        columns: x => new { x.PlayerSeasonPlayerId, x.PlayerSeasonSeasonId },
                        principalTable: "PlayerSeasons",
                        principalColumns: new[] { "PlayerId", "SeasonId" });
                    table.ForeignKey(
                        name: "FK_Matches_Players_BlackPlayerId",
                        column: x => x.BlackPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matches_Players_WhitePlayerId",
                        column: x => x.WhitePlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matches_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Rate = table.Column<float>(type: "real", nullable: false),
                    MaxRank = table.Column<int>(type: "integer", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    MailAddress = table.Column<string>(type: "text", nullable: false),
                    DiscordHandle = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MatchId = table.Column<int>(type: "integer", nullable: true),
                    ReviewStatus = table.Column<int>(type: "integer", nullable: false),
                    OwnerPlayerId = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: true),
                    Round = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_Players_OwnerPlayerId",
                        column: x => x.OwnerPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_BlackPlayerId",
                table: "Matches",
                column: "BlackPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_PlayerSeasonPlayerId_PlayerSeasonSeasonId",
                table: "Matches",
                columns: new[] { "PlayerSeasonPlayerId", "PlayerSeasonSeasonId" });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_SeasonId",
                table: "Matches",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_WhitePlayerId",
                table: "Matches",
                column: "WhitePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MatchId",
                table: "Reviews",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OwnerPlayerId",
                table: "Reviews",
                column: "OwnerPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TeacherId",
                table: "Reviews",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropColumn(
                name: "LeagoKey",
                table: "Players");
        }
    }
}
