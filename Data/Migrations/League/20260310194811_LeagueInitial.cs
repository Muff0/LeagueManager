using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations.League
{
    /// <inheritdoc />
    public partial class LeagueInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    EmailAddress = table.Column<string>(type: "text", nullable: false),
                    DiscordHandle = table.Column<string>(type: "text", nullable: false),
                    OGSHandle = table.Column<string>(type: "text", nullable: false),
                    LeagoMemberId = table.Column<string>(type: "text", nullable: false),
                    GoMagicUserId = table.Column<int>(type: "integer", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    LeagoKey = table.Column<string>(type: "text", nullable: false),
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    Timezone = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    LeagoL1Key = table.Column<string>(type: "text", nullable: false),
                    LeagoL2Key = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
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
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerSeasons",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    SeasonId = table.Column<int>(type: "integer", nullable: false),
                    ParticipationTier = table.Column<int>(type: "integer", nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSeasons", x => new { x.PlayerId, x.SeasonId });
                    table.ForeignKey(
                        name: "FK_PlayerSeasons_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerSeasons_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MatchUrl = table.Column<string>(type: "text", nullable: false),
                    IsComplete = table.Column<bool>(type: "boolean", nullable: false),
                    Round = table.Column<int>(type: "integer", nullable: false),
                    SeasonId = table.Column<int>(type: "integer", nullable: false),
                    NotificationSent = table.Column<bool>(type: "boolean", nullable: false),
                    GameTimeUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LeagoKey = table.Column<string>(type: "text", nullable: false),
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
                        name: "FK_Matches_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerMatches",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    MatchId = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<int>(type: "integer", nullable: false),
                    HasConfirmed = table.Column<bool>(type: "boolean", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MatchId = table.Column<int>(type: "integer", nullable: true),
                    ReviewStatus = table.Column<int>(type: "integer", nullable: false),
                    OwnerPlayerId = table.Column<int>(type: "integer", nullable: false),
                    SeasonId = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: true),
                    Round = table.Column<int>(type: "integer", nullable: false),
                    ReviewUrl = table.Column<string>(type: "text", nullable: false)
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
                        name: "FK_Reviews_PlayerSeasons_OwnerPlayerId_SeasonId",
                        columns: x => new { x.OwnerPlayerId, x.SeasonId },
                        principalTable: "PlayerSeasons",
                        principalColumns: new[] { "PlayerId", "SeasonId" },
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "ReviewSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewId = table.Column<int>(type: "integer", nullable: false),
                    UTCSchedule = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DiscordEventId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewSchedules_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_LeagoKey",
                table: "Matches",
                column: "LeagoKey");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_PlayerSeasonPlayerId_PlayerSeasonSeasonId",
                table: "Matches",
                columns: new[] { "PlayerSeasonPlayerId", "PlayerSeasonSeasonId" });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_SeasonId",
                table: "Matches",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatches_MatchId",
                table: "PlayerMatches",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSeasons_SeasonId",
                table: "PlayerSeasons",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MatchId",
                table: "Reviews",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OwnerPlayerId_SeasonId",
                table: "Reviews",
                columns: new[] { "OwnerPlayerId", "SeasonId" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TeacherId",
                table: "Reviews",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewSchedules_ReviewId",
                table: "ReviewSchedules",
                column: "ReviewId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerMatches");

            migrationBuilder.DropTable(
                name: "ReviewSchedules");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "PlayerSeasons");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Seasons");
        }
    }
}
