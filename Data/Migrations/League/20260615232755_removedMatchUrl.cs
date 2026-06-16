using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.League
{
    /// <inheritdoc />
    public partial class removedMatchUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchUrl",
                table: "Matches");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MatchUrl",
                table: "Matches",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
