using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class Addannouncement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Users_PvetAdminId",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_PvetAdminId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "PvetAdminId",
                table: "Announcements");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Announcements",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Announcements");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Announcements",
                newName: "Date");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Announcements",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "PvetAdminId",
                table: "Announcements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_PvetAdminId",
                table: "Announcements",
                column: "PvetAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Users_PvetAdminId",
                table: "Announcements",
                column: "PvetAdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
