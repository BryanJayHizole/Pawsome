using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class EditUserProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "Middlename");

            migrationBuilder.RenameColumn(
                name: "Middleinitial",
                table: "Users",
                newName: "Gender");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Middlename",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Users",
                newName: "Middleinitial");
        }
    }
}
