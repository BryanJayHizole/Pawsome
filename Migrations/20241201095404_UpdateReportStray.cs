using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReportStray : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncidentDescription",
                table: "StrayReports");

            migrationBuilder.DropColumn(
                name: "IncidentType",
                table: "StrayReports");

            migrationBuilder.RenameColumn(
                name: "IsIncident",
                table: "StrayReports",
                newName: "IsStray");

            migrationBuilder.AddColumn<string>(
                name: "OwnerContact",
                table: "StrayReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "StrayReports",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "StrayReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PetId",
                table: "StrayReports",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PetName",
                table: "StrayReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StrayReports_PetId",
                table: "StrayReports",
                column: "PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_StrayReports_Pets_PetId",
                table: "StrayReports",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StrayReports_Pets_PetId",
                table: "StrayReports");

            migrationBuilder.DropIndex(
                name: "IX_StrayReports_PetId",
                table: "StrayReports");

            migrationBuilder.DropColumn(
                name: "OwnerContact",
                table: "StrayReports");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "StrayReports");

            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "StrayReports");

            migrationBuilder.DropColumn(
                name: "PetId",
                table: "StrayReports");

            migrationBuilder.DropColumn(
                name: "PetName",
                table: "StrayReports");

            migrationBuilder.RenameColumn(
                name: "IsStray",
                table: "StrayReports",
                newName: "IsIncident");

            migrationBuilder.AddColumn<string>(
                name: "IncidentDescription",
                table: "StrayReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncidentType",
                table: "StrayReports",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
