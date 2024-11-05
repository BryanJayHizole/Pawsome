using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    /// <inheritdoc />
    public partial class DropUserid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint for UserId
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_UserId", // Name of the foreign key constraint
                table: "Appointments" // Table from which to drop the constraint
            );

            // Drop the index associated with UserId
            migrationBuilder.DropIndex(
                name: "IX_Appointments_UserId", // Name of the index to drop
                table: "Appointments" // Table from which to drop the index
            );

            

            // Drop the index associated with PvetAdminId
            migrationBuilder.DropIndex(
                name: "IX_Appointments_PvetAdminId", // Name of the index to drop
                table: "Appointments" // Table from which to drop the index
            );

            // Drop the UserId column from the Appointments table
            migrationBuilder.DropColumn(
                name: "UserId", // Name of the column to drop
                table: "Appointments" // Name of the table
            );

            // Drop the PvetAdminId column from the Appointments table
            migrationBuilder.DropColumn(
                name: "PvetAdminId", // Name of the column to drop
                table: "Appointments" // Name of the table
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add the UserId column back to the Appointments table
            migrationBuilder.AddColumn<int>(
                name: "UserId", // Name of the column to add back
                table: "Appointments", // Name of the table
                type: "int", // Type of the column
                nullable: true // Allow null values for the column
            );

            // Recreate the index for UserId
            migrationBuilder.CreateIndex(
                name: "IX_Appointments_UserId", // Name of the index to create
                table: "Appointments", // Table on which to create the index
                column: "UserId" // Column for the index
            );

            // Add the foreign key constraint back
            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_UserId", // Name of the foreign key constraint
                table: "Appointments", // Table to which the constraint applies
                column: "UserId", // Column that holds the foreign key
                principalTable: "Users", // Referenced table
                principalColumn: "Id", // Referenced column
                onDelete: ReferentialAction.Restrict // Delete behavior
            );

            // Add the PvetAdminId column back to the Appointments table
            migrationBuilder.AddColumn<int>(
                name: "PvetAdminId", // Name of the column to add back
                table: "Appointments", // Name of the table
                type: "int", // Type of the column
                nullable: true // Allow null values for the column
            );

            // Recreate the index for PvetAdminId
            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PvetAdminId", // Name of the index to create
                table: "Appointments", // Table on which to create the index
                column: "PvetAdminId" // Column for the index
            );

            
        }
    }
}
