using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class VSourceInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VaccineSourceId",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_VaccineSourceId",
                table: "InventoryItems",
                column: "VaccineSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_VaccineSources_VaccineSourceId",
                table: "InventoryItems",
                column: "VaccineSourceId",
                principalTable: "VaccineSources",
                principalColumn: "VSourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_VaccineSources_VaccineSourceId",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_VaccineSourceId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "VaccineSourceId",
                table: "InventoryItems");
        }
    }
}
