using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class VaccineRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VaccinationHistories", // Change to plural
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VaccinationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VaccineType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VaccinationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdministeredBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextDueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PetId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaccinationHistories", x => x.Id); // Change to plural
                    table.ForeignKey(
                        name: "FK_VaccinationHistories_Pets_PetId", // Change to plural
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationHistories_PetId", // Change to plural
                table: "VaccinationHistories",
                column: "PetId");
        }

       
    }
}
