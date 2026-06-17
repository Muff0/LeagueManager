using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.League
{
    /// <inheritdoc />
    public partial class OgsInviteLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OgsInviteLink",
                table: "PlayerMatches",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OgsLeagueMatchId",
                table: "Matches",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OgsInviteLink",
                table: "PlayerMatches");

            migrationBuilder.DropColumn(
                name: "OgsLeagueMatchId",
                table: "Matches");
        }
    }
}
