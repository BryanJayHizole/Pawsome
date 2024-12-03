using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class SystemSettingv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BarangayId",
                table: "SystemSettings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_BarangayId",
                table: "SystemSettings",
                column: "BarangayId");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemSettings_Barangays_BarangayId",
                table: "SystemSettings",
                column: "BarangayId",
                principalTable: "Barangays",
                principalColumn: "BarangayId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemSettings_Barangays_BarangayId",
                table: "SystemSettings");

            migrationBuilder.DropIndex(
                name: "IX_SystemSettings_BarangayId",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "BarangayId",
                table: "SystemSettings");
        }
    }
}
