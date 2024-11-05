using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Announcements");

            migrationBuilder.AddColumn<string>(
                name: "Photos",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photos",
                table: "Announcements");

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "Announcements",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
