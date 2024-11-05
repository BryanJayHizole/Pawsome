using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class AlterUserPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Photo",
                table: "Users",
                nullable: true,  // Set nullable to true
                defaultValue: null,  // Remove the default value
                oldClrType: typeof(byte[]),  // Specify the old CLR type
                oldType: "varbinary(max)",  // Specify the old column type
                oldNullable: false);  // Specify the old nullable property



        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Photo",
                table: "Users",
                nullable: false,  // Set nullable back to false
                oldClrType: typeof(byte[]),  // Specify the old CLR type
                oldType: "varbinary(max)",  // Specify the old column type
                oldNullable: true);  // Specify the old nullable property
        }
    }
}
