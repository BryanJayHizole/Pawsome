using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidentToStrayReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<bool>(
                name: "IsIncident",
                table: "StrayReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Purok",
                table: "StrayReports",
                type: "nvarchar(max)",
                nullable: true);



        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncidentDescription",
                table: "StrayReports");

            migrationBuilder.DropColumn(
                name: "IncidentType",
                table: "StrayReports");

            migrationBuilder.DropColumn(
                name: "IsIncident",
                table: "StrayReports");

            migrationBuilder.DropColumn(
                name: "Purok",
                table: "StrayReports");

          
        }
    }
}
