using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    public partial class AddBreed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the Breeds table
            migrationBuilder.CreateTable(
                name: "Breeds",
                columns: table => new
                {
                    BreedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"), // Auto-increment column
                    BreedType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false) // Foreign Key to PetTypes
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breeds", x => x.BreedId);
                    // Foreign Key Constraint to PetTypes
                    table.ForeignKey(
                        name: "FK_Breeds_PetTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "PetTypes",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Cascade); // When a PetType is deleted, related Breeds are deleted
                });

            // Optional: If you want to add data to the Breeds table, you can insert data here
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the Breeds table in the Down method to reverse the migration
            migrationBuilder.DropTable(
                name: "Breeds");
        }
    }
}
