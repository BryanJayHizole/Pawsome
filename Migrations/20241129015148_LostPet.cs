using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class LostPet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PetTagNo",
                table: "LostPetReports", // Make sure to replace "Pets" with the actual table name
                type: "nvarchar(max)", // Adjust the type if necessary
                nullable: true, // This allows the column to accept null values
                oldClrType: typeof(string), // Previous type
                oldType: "nvarchar(max)", // Previous type (if applicable)
                oldNullable: false); // Previous nullable state
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
