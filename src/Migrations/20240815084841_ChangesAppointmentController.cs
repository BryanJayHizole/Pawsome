using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class ChangesAppointmentController : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SchedulingAppointments_PetId",
                table: "SchedulingAppointments",
                column: "PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchedulingAppointments_Pets_PetId",
                table: "SchedulingAppointments",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchedulingAppointments_Pets_PetId",
                table: "SchedulingAppointments");

            migrationBuilder.DropIndex(
                name: "IX_SchedulingAppointments_PetId",
                table: "SchedulingAppointments");
        }
    }
}
