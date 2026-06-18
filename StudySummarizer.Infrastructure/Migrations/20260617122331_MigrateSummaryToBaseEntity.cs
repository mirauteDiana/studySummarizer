using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySummarizer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrateSummaryToBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GeneratedAt",
                table: "Summaries",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Summaries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Summaries");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Summaries",
                newName: "GeneratedAt");
        }
    }
}
