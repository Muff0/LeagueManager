using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class _0002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSeason_Players_PlayerId",
                table: "PlayerSeason");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSeason_Season_SeasonId",
                table: "PlayerSeason");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Season",
                table: "Season");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerSeason",
                table: "PlayerSeason");

            migrationBuilder.RenameTable(
                name: "Season",
                newName: "Seasons");

            migrationBuilder.RenameTable(
                name: "PlayerSeason",
                newName: "PlayerSeasons");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerSeason_SeasonId",
                table: "PlayerSeasons",
                newName: "IX_PlayerSeasons_SeasonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Seasons",
                table: "Seasons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerSeasons",
                table: "PlayerSeasons",
                columns: new[] { "PlayerId", "SeasonId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerSeasons_Players_PlayerId",
                table: "PlayerSeasons",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerSeasons_Seasons_SeasonId",
                table: "PlayerSeasons",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSeasons_Players_PlayerId",
                table: "PlayerSeasons");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSeasons_Seasons_SeasonId",
                table: "PlayerSeasons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Seasons",
                table: "Seasons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerSeasons",
                table: "PlayerSeasons");

            migrationBuilder.RenameTable(
                name: "Seasons",
                newName: "Season");

            migrationBuilder.RenameTable(
                name: "PlayerSeasons",
                newName: "PlayerSeason");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerSeasons_SeasonId",
                table: "PlayerSeason",
                newName: "IX_PlayerSeason_SeasonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Season",
                table: "Season",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerSeason",
                table: "PlayerSeason",
                columns: new[] { "PlayerId", "SeasonId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerSeason_Players_PlayerId",
                table: "PlayerSeason",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerSeason_Season_SeasonId",
                table: "PlayerSeason",
                column: "SeasonId",
                principalTable: "Season",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
