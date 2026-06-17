using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySummarizer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UploadDate",
                table: "Documents",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Documents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Documents",
                newName: "UploadDate");
        }
    }
}
