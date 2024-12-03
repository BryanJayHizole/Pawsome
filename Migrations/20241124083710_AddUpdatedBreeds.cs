using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    public partial class AddUpdatedBreeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the Breeds table
            migrationBuilder.CreateTable(
                name: "Breeds",
                columns: table => new
                {
                    BreedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),  // Auto-increment for BreedId
                    BreedName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false) // Foreign key column for PetTypes
                },
                constraints: table =>
                {
                    // Primary key constraint
                    table.PrimaryKey("PK_Breeds", x => x.BreedId);

                    // Foreign key constraint to link PetTypeId in Breeds to TypeId in PetTypes
                    table.ForeignKey(
                        name: "FK_Breeds_PetTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "PetTypes",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Cascade);  // You can change to Restrict if needed
                });

            // Add an index on PetTypeId for better performance on foreign key lookups
            migrationBuilder.CreateIndex(
                name: "IX_Breeds_TypeId",
                table: "Breeds",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the Breeds table if rolling back
            migrationBuilder.DropTable(
                name: "Breeds");
        }
    }
}
