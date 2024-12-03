using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedIncident : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "RabiesIncidents",
                newName: "TypeofIncident");

            migrationBuilder.AddColumn<string>(
                name: "Breed",
                table: "RabiesIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "RabiesIncidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "RabiesIncidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsStray",
                table: "RabiesIncidents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextDueDate",
                table: "RabiesIncidents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerContact",
                table: "RabiesIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "RabiesIncidents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "RabiesIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PetId",
                table: "RabiesIncidents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PetName",
                table: "RabiesIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PetPhoto",
                table: "RabiesIncidents",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PetType",
                table: "RabiesIncidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Purok",
                table: "RabiesIncidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "RabiesIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagNo",
                table: "RabiesIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaccinatedBy",
                table: "RabiesIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VaccinationDate",
                table: "RabiesIncidents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaccinationStatus",
                table: "RabiesIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VaccineBlockNo",
                table: "RabiesIncidents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaccineType",
                table: "RabiesIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RabiesIncidents_PetId",
                table: "RabiesIncidents",
                column: "PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_RabiesIncidents_Pets_PetId",
                table: "RabiesIncidents",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RabiesIncidents_Pets_PetId",
                table: "RabiesIncidents");

            migrationBuilder.DropIndex(
                name: "IX_RabiesIncidents_PetId",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "Breed",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "IsStray",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "NextDueDate",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "OwnerContact",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "PetId",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "PetName",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "PetPhoto",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "PetType",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "Purok",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "TagNo",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "VaccinatedBy",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "VaccinationDate",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "VaccinationStatus",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "VaccineBlockNo",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "VaccineType",
                table: "RabiesIncidents");

            migrationBuilder.RenameColumn(
                name: "TypeofIncident",
                table: "RabiesIncidents",
                newName: "Type");
        }
    }
}
