using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePetVaccinationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alter the columns to allow null values
            migrationBuilder.AlterColumn<string>(
                name: "VaccineType",
                table: "Pets",
                nullable: true, // Allow null values
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "VaccinationDate",
                table: "Pets",
                nullable: true, // Allow null values
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "VaccinatedBy",
                table: "Pets",
                nullable: true, // Allow null values
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "VaccineBlockNo",
                table: "Pets",
                nullable: true, // Allow null values
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "VaccineSource",
                table: "Pets",
                nullable: true, // Allow null values
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert the columns to not allow null values
            migrationBuilder.AlterColumn<string>(
                name: "VaccineType",
                table: "Pets",
                nullable: false, // Disallow null values
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "VaccinationDate",
                table: "Pets",
                nullable: false, // Disallow null values
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VaccinatedBy",
                table: "Pets",
                nullable: false, // Disallow null values
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VaccineBlockNo",
                table: "Pets",
                nullable: false, // Disallow null values
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VaccineSource",
                table: "Pets",
                nullable: false, // Disallow null values
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
