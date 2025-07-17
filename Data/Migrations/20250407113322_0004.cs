using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class _0004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Seasons",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BlackConfirmed",
                table: "Matches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "GameTime",
                table: "Matches",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeagoKey",
                table: "Matches",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "NotificationSent",
                table: "Matches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WhiteConfirmed",
                table: "Matches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_LeagoKey",
                table: "Matches",
                column: "LeagoKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Matches_LeagoKey",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "BlackConfirmed",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "GameTime",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "LeagoKey",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "NotificationSent",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "WhiteConfirmed",
                table: "Matches");
        }
    }
}