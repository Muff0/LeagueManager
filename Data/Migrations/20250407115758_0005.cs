using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class _0005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LeagoKey",
                table: "Seasons",
                newName: "LeagoL2Key");

            migrationBuilder.AddColumn<string>(
                name: "LeagoL1Key",
                table: "Seasons",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeagoL1Key",
                table: "Seasons");

            migrationBuilder.RenameColumn(
                name: "LeagoL2Key",
                table: "Seasons",
                newName: "LeagoKey");
        }
    }
}