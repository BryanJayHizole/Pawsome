using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class InitialBarangayAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAdmin",
                table: "Users",
                newName: "IsBarangayAdmin");

            migrationBuilder.AddColumn<string>(
                name: "PetBarangay",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PetCity",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PetProvince",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PetBarangay",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "PetCity",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "PetProvince",
                table: "Pets");

            migrationBuilder.RenameColumn(
                name: "IsBarangayAdmin",
                table: "Users",
                newName: "IsAdmin");
        }
    }
}
