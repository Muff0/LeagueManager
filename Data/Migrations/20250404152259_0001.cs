using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class _0001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Season",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    LeagoKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Season", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerSeason",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    SeasonId = table.Column<int>(type: "integer", nullable: false),
                    ParticipationTier = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSeason", x => new { x.PlayerId, x.SeasonId });
                    table.ForeignKey(
                        name: "FK_PlayerSeason_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerSeason_Season_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Season",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSeason_SeasonId",
                table: "PlayerSeason",
                column: "SeasonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerSeason");

            migrationBuilder.DropTable(
                name: "Season");
        }
    }
}
