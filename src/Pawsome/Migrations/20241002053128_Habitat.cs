using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class Habitat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Habitats",
                columns: table => new
                {
                    HabitatId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),  // Auto-incrementing primary key
                    HabitatType = table.Column<string>(nullable: true)  // Nullable string column for HabitatType
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habitats", x => x.HabitatId);  // Define primary key
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Habitats");  // Drop the table when rolling back the migration
        }
    }
}
