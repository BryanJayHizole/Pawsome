using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    public partial class RenameBreedType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename column BreedName to BreedType
            migrationBuilder.RenameColumn(
                name: "BreedName",       // Old column name
                table: "Breeds",        // Table name
                newName: "BreedType");  // New column name
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rename column BreedType back to BreedName in case of rollback
            migrationBuilder.RenameColumn(
                name: "BreedType",      // New column name
                table: "Breeds",        // Table name
                newName: "BreedName");  // Old column name
        }
    }
}
