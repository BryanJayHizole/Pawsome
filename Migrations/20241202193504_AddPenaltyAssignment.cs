using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class AddPenaltyAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Pets");

            migrationBuilder.AddColumn<int>(
                name: "PetId",
                table: "PenaltyFines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PenaltyAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PenaltyFineId = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PenaltyAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PenaltyAssignments_PenaltyFines_PenaltyFineId",
                        column: x => x.PenaltyFineId,
                        principalTable: "PenaltyFines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PenaltyAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyFines_PetId",
                table: "PenaltyFines",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyAssignments_PenaltyFineId",
                table: "PenaltyAssignments",
                column: "PenaltyFineId");

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyAssignments_UserId",
                table: "PenaltyAssignments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PenaltyFines_Pets_PetId",
                table: "PenaltyFines",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PenaltyFines_Pets_PetId",
                table: "PenaltyFines");

            migrationBuilder.DropTable(
                name: "PenaltyAssignments");

            migrationBuilder.DropIndex(
                name: "IX_PenaltyFines_PetId",
                table: "PenaltyFines");

            migrationBuilder.DropColumn(
                name: "PetId",
                table: "PenaltyFines");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Pets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
