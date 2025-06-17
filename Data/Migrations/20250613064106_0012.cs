using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class _0012 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_OwnerPlayerId",
                table: "Reviews");

            migrationBuilder.AddColumn<int>(
                name: "SeasonId",
                table: "Reviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OwnerPlayerId_SeasonId",
                table: "Reviews",
                columns: new[] { "OwnerPlayerId", "SeasonId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_PlayerSeasons_OwnerPlayerId_SeasonId",
                table: "Reviews",
                columns: new[] { "OwnerPlayerId", "SeasonId" },
                principalTable: "PlayerSeasons",
                principalColumns: new[] { "PlayerId", "SeasonId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_PlayerSeasons_OwnerPlayerId_SeasonId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_OwnerPlayerId_SeasonId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "SeasonId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OwnerPlayerId",
                table: "Reviews",
                column: "OwnerPlayerId");
        }
    }
}