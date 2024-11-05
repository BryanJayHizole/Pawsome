using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class RabiesPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RabiesIncidents_Users_ReportedByUserId",
                table: "RabiesIncidents");

            migrationBuilder.DropIndex(
                name: "IX_RabiesIncidents_ReportedByUserId",
                table: "RabiesIncidents");

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "RabiesIncidents",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportedByUser",
                table: "RabiesIncidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "RabiesIncidents");

            migrationBuilder.DropColumn(
                name: "ReportedByUser",
                table: "RabiesIncidents");

            migrationBuilder.CreateIndex(
                name: "IX_RabiesIncidents_ReportedByUserId",
                table: "RabiesIncidents",
                column: "ReportedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RabiesIncidents_Users_ReportedByUserId",
                table: "RabiesIncidents",
                column: "ReportedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
