#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations.League;

/// <inheritdoc />
public partial class LeagueInitial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "Players",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                FirstName = table.Column<string>("text", nullable: false),
                LastName = table.Column<string>("text", nullable: false),
                EmailAddress = table.Column<string>("text", nullable: false),
                DiscordHandle = table.Column<string>("text", nullable: false),
                OGSHandle = table.Column<string>("text", nullable: false),
                LeagoMemberId = table.Column<string>("text", nullable: false),
                GoMagicUserId = table.Column<int>("integer", nullable: false),
                Rank = table.Column<int>("integer", nullable: false),
                LeagoKey = table.Column<string>("text", nullable: false),
                DiscordId = table.Column<decimal>("numeric(20,0)", nullable: true),
                Timezone = table.Column<string>("text", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Players", x => x.Id); });

        migrationBuilder.CreateTable(
            "Seasons",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Title = table.Column<string>("text", nullable: false),
                LeagoL1Key = table.Column<string>("text", nullable: false),
                LeagoL2Key = table.Column<string>("text", nullable: false),
                IsActive = table.Column<bool>("boolean", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Seasons", x => x.Id); });

        migrationBuilder.CreateTable(
            "Teachers",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>("text", nullable: false),
                Rate = table.Column<float>("real", nullable: false),
                MaxRank = table.Column<int>("integer", nullable: false),
                Rank = table.Column<int>("integer", nullable: false),
                MailAddress = table.Column<string>("text", nullable: false),
                DiscordId = table.Column<decimal>("numeric(20,0)", nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_Teachers", x => x.Id); });

        migrationBuilder.CreateTable(
            "PlayerSeasons",
            table => new
            {
                PlayerId = table.Column<int>("integer", nullable: false),
                SeasonId = table.Column<int>("integer", nullable: false),
                ParticipationTier = table.Column<int>("integer", nullable: false),
                PaymentStatus = table.Column<int>("integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PlayerSeasons", x => new { x.PlayerId, x.SeasonId });
                table.ForeignKey(
                    "FK_PlayerSeasons_Players_PlayerId",
                    x => x.PlayerId,
                    "Players",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_PlayerSeasons_Seasons_SeasonId",
                    x => x.SeasonId,
                    "Seasons",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Matches",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                MatchUrl = table.Column<string>("text", nullable: false),
                IsComplete = table.Column<bool>("boolean", nullable: false),
                Round = table.Column<int>("integer", nullable: false),
                SeasonId = table.Column<int>("integer", nullable: false),
                NotificationSent = table.Column<bool>("boolean", nullable: false),
                GameTimeUTC = table.Column<DateTime>("timestamp with time zone", nullable: true),
                LeagoKey = table.Column<string>("text", nullable: false),
                PlayerSeasonPlayerId = table.Column<int>("integer", nullable: true),
                PlayerSeasonSeasonId = table.Column<int>("integer", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Matches", x => x.Id);
                table.ForeignKey(
                    "FK_Matches_PlayerSeasons_PlayerSeasonPlayerId_PlayerSeasonSeas~",
                    x => new { x.PlayerSeasonPlayerId, x.PlayerSeasonSeasonId },
                    "PlayerSeasons",
                    new[] { "PlayerId", "SeasonId" });
                table.ForeignKey(
                    "FK_Matches_Seasons_SeasonId",
                    x => x.SeasonId,
                    "Seasons",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "PlayerMatches",
            table => new
            {
                PlayerId = table.Column<int>("integer", nullable: false),
                MatchId = table.Column<int>("integer", nullable: false),
                Color = table.Column<int>("integer", nullable: false),
                HasConfirmed = table.Column<bool>("boolean", nullable: false),
                Outcome = table.Column<int>("integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PlayerMatches", x => new { x.PlayerId, x.MatchId });
                table.ForeignKey(
                    "FK_PlayerMatches_Matches_MatchId",
                    x => x.MatchId,
                    "Matches",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_PlayerMatches_Players_PlayerId",
                    x => x.PlayerId,
                    "Players",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Reviews",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                MatchId = table.Column<int>("integer", nullable: true),
                ReviewStatus = table.Column<int>("integer", nullable: false),
                OwnerPlayerId = table.Column<int>("integer", nullable: false),
                SeasonId = table.Column<int>("integer", nullable: false),
                TeacherId = table.Column<int>("integer", nullable: true),
                Round = table.Column<int>("integer", nullable: false),
                ReviewUrl = table.Column<string>("text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Reviews", x => x.Id);
                table.ForeignKey(
                    "FK_Reviews_Matches_MatchId",
                    x => x.MatchId,
                    "Matches",
                    "Id");
                table.ForeignKey(
                    "FK_Reviews_PlayerSeasons_OwnerPlayerId_SeasonId",
                    x => new { x.OwnerPlayerId, x.SeasonId },
                    "PlayerSeasons",
                    new[] { "PlayerId", "SeasonId" },
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_Reviews_Players_OwnerPlayerId",
                    x => x.OwnerPlayerId,
                    "Players",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_Reviews_Teachers_TeacherId",
                    x => x.TeacherId,
                    "Teachers",
                    "Id");
            });

        migrationBuilder.CreateTable(
            "ReviewSchedules",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ReviewId = table.Column<int>("integer", nullable: false),
                UTCSchedule = table.Column<DateTime>("timestamp with time zone", nullable: false),
                DiscordEventId = table.Column<decimal>("numeric(20,0)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ReviewSchedules", x => x.Id);
                table.ForeignKey(
                    "FK_ReviewSchedules_Reviews_ReviewId",
                    x => x.ReviewId,
                    "Reviews",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_Matches_LeagoKey",
            "Matches",
            "LeagoKey");

        migrationBuilder.CreateIndex(
            "IX_Matches_PlayerSeasonPlayerId_PlayerSeasonSeasonId",
            "Matches",
            new[] { "PlayerSeasonPlayerId", "PlayerSeasonSeasonId" });

        migrationBuilder.CreateIndex(
            "IX_Matches_SeasonId",
            "Matches",
            "SeasonId");

        migrationBuilder.CreateIndex(
            "IX_PlayerMatches_MatchId",
            "PlayerMatches",
            "MatchId");

        migrationBuilder.CreateIndex(
            "IX_PlayerSeasons_SeasonId",
            "PlayerSeasons",
            "SeasonId");

        migrationBuilder.CreateIndex(
            "IX_Reviews_MatchId",
            "Reviews",
            "MatchId");

        migrationBuilder.CreateIndex(
            "IX_Reviews_OwnerPlayerId_SeasonId",
            "Reviews",
            new[] { "OwnerPlayerId", "SeasonId" });

        migrationBuilder.CreateIndex(
            "IX_Reviews_TeacherId",
            "Reviews",
            "TeacherId");

        migrationBuilder.CreateIndex(
            "IX_ReviewSchedules_ReviewId",
            "ReviewSchedules",
            "ReviewId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "PlayerMatches");

        migrationBuilder.DropTable(
            "ReviewSchedules");

        migrationBuilder.DropTable(
            "Reviews");

        migrationBuilder.DropTable(
            "Matches");

        migrationBuilder.DropTable(
            "Teachers");

        migrationBuilder.DropTable(
            "PlayerSeasons");

        migrationBuilder.DropTable(
            "Players");

        migrationBuilder.DropTable(
            "Seasons");
    }
}