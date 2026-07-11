using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportGenerator.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFileUrlToReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "Reports",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "Reports");
        }
    }
}
