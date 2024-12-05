using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDicPenal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BarangayPenalties");

            migrationBuilder.AddColumn<string>(
                name: "BarangayName",
                table: "PenaltyFines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarangayName",
                table: "PenaltyFines");

            migrationBuilder.CreateTable(
                name: "BarangayPenalties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PenaltyFineId = table.Column<int>(type: "int", nullable: false),
                    BarangayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FineAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarangayPenalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BarangayPenalties_PenaltyFines_PenaltyFineId",
                        column: x => x.PenaltyFineId,
                        principalTable: "PenaltyFines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BarangayPenalties_PenaltyFineId",
                table: "BarangayPenalties",
                column: "PenaltyFineId");
        }
    }
}
