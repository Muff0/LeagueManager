using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class _0016 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscordHandle",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "MatchOutcome",
                table: "PlayerMatches");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscordId",
                table: "Teachers",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "GameTimeUTC",
                table: "Matches",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscordId",
                table: "Teachers");

            migrationBuilder.AddColumn<string>(
                name: "DiscordHandle",
                table: "Teachers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MatchOutcome",
                table: "PlayerMatches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "GameTimeUTC",
                table: "Matches",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
