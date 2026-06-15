using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.League
{
    /// <inheritdoc />
    public partial class LeagueMatchId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {migrationBuilder.Sql(
                "UPDATE \"Matches\" SET \"OgsLeagueMatchId\" = '0' WHERE \"OgsLeagueMatchId\" IS NULL OR \"OgsLeagueMatchId\" = ''");
            migrationBuilder.Sql(
                "ALTER TABLE \"Matches\" ALTER COLUMN \"OgsLeagueMatchId\" DROP DEFAULT");
            migrationBuilder.Sql(
                "ALTER TABLE \"Matches\" ALTER COLUMN \"OgsLeagueMatchId\" TYPE integer USING \"OgsLeagueMatchId\"::integer");
            migrationBuilder.Sql(
                "ALTER TABLE \"Matches\" ALTER COLUMN \"OgsLeagueMatchId\" SET DEFAULT 0");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {migrationBuilder.Sql(
            @"ALTER TABLE ""Matches""
      ALTER COLUMN ""OgsLeagueMatchId"" TYPE text
      USING ""OgsLeagueMatchId""::text");
        }
    }
}
