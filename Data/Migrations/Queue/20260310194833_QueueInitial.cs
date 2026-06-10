#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations.Queue;

/// <inheritdoc />
public partial class QueueInitial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "CommandQueue",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Status = table.Column<int>("integer", nullable: false),
                Type = table.Column<string>("text", nullable: false),
                Payload = table.Column<string>("text", nullable: false),
                CreatedAtUtc = table.Column<DateTime>("timestamp with time zone", nullable: false),
                ProcessedAtUtc = table.Column<DateTime>("timestamp with time zone", nullable: false),
                Retries = table.Column<int>("integer", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_CommandQueue", x => x.Id); });

        migrationBuilder.CreateTable(
            "EventQueue",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Status = table.Column<int>("integer", nullable: false),
                Type = table.Column<string>("text", nullable: false),
                Payload = table.Column<string>("text", nullable: false),
                CreatedAtUtc = table.Column<DateTime>("timestamp with time zone", nullable: false),
                ProcessedAtUtc = table.Column<DateTime>("timestamp with time zone", nullable: false),
                CorrelationId = table.Column<int>("integer", nullable: false),
                Retries = table.Column<int>("integer", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_EventQueue", x => x.Id); });

        migrationBuilder.CreateTable(
            "MessageQueue",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Status = table.Column<int>("integer", nullable: false),
                Type = table.Column<string>("text", nullable: false),
                Payload = table.Column<string>("text", nullable: false),
                CorrelationId = table.Column<int>("integer", nullable: false),
                CreatedAtUtc = table.Column<DateTime>("timestamp with time zone", nullable: false),
                ProcessedAtUtc = table.Column<DateTime>("timestamp with time zone", nullable: false),
                Retries = table.Column<int>("integer", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_MessageQueue", x => x.Id); });

        migrationBuilder.CreateIndex(
            "IX_CommandQueue_Status",
            "CommandQueue",
            "Status");

        migrationBuilder.CreateIndex(
            "IX_EventQueue_Status",
            "EventQueue",
            "Status");

        migrationBuilder.CreateIndex(
            "IX_MessageQueue_Status",
            "MessageQueue",
            "Status");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "CommandQueue");

        migrationBuilder.DropTable(
            "EventQueue");

        migrationBuilder.DropTable(
            "MessageQueue");
    }
}