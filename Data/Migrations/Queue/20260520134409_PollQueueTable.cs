#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations.Queue;

/// <inheritdoc />
public partial class PollQueueTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "PollQueue",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Status = table.Column<int>("integer", nullable: false),
                Payload = table.Column<string>("text", nullable: false),
                CreatedAtUtc = table.Column<DateTime>("timestamp with time zone", nullable: false),
                ProcessedAtUtc = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_PollQueue", x => x.Id); });

        migrationBuilder.CreateIndex(
            "IX_PollQueue_Status",
            "PollQueue",
            "Status");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "PollQueue");
    }
}