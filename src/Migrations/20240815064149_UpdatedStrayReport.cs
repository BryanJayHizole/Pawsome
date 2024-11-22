using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedStrayReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StrayBreed",
                table: "StrayReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StrayColor",
                table: "StrayReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StrayGender",
                table: "StrayReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StrayTag",
                table: "StrayReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StrayTagNo",
                table: "StrayReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StrayType",
                table: "StrayReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StrayBreed",
                table: "StrayReports");

            migrationBuilder.DropColumn(
                name: "StrayColor",
                table: "StrayReports");

            migrationBuilder.DropColumn(
                name: "StrayGender",
                table: "StrayReports");

            migrationBuilder.DropColumn(
                name: "StrayTag",
                table: "StrayReports");

            migrationBuilder.DropColumn(
                name: "StrayTagNo",
                table: "StrayReports");

            migrationBuilder.DropColumn(
                name: "StrayType",
                table: "StrayReports");
        }
    }
}
