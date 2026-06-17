using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Queue
{
    /// <inheritdoc />
    public partial class RenamedDateTimeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SettingsUpdatedAt",
                table: "JobRegistry",
                newName: "SettingsUpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "LastRunAt",
                table: "JobRegistry",
                newName: "LastRunAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SettingsUpdatedAtUtc",
                table: "JobRegistry",
                newName: "SettingsUpdatedAt");

            migrationBuilder.RenameColumn(
                name: "LastRunAtUtc",
                table: "JobRegistry",
                newName: "LastRunAt");
        }
    }
}
