using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class VaccineSourceOnVaccine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "VaccineTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "VaccineSourceId",
                table: "VaccineTypes",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_VaccineTypes_VaccineSourceId",
                table: "VaccineTypes",
                column: "VaccineSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_VaccineTypes_VaccineSources_VaccineSourceId",
                table: "VaccineTypes",
                column: "VaccineSourceId",
                principalTable: "VaccineSources",
                principalColumn: "VSourceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VaccineTypes_VaccineSources_VaccineSourceId",
                table: "VaccineTypes");

            migrationBuilder.DropIndex(
                name: "IX_VaccineTypes_VaccineSourceId",
                table: "VaccineTypes");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "VaccineTypes");

            migrationBuilder.DropColumn(
                name: "VaccineSourceId",
                table: "VaccineTypes");
        }
    }
}
