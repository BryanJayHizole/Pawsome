using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Migrations
{
    public partial class RearrangeUserColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create a new temporary table with the desired column order
            migrationBuilder.CreateTable(
                name: "TempUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false),
                    Firstname = table.Column<string>(nullable: true),
                    Middlename = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    Age = table.Column<int>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: true),
                    ContactNumber = table.Column<string>(nullable: true),
                    Barangay = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    ProfilePhoto = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempUsers", x => x.Id);
                });

            // Enable IDENTITY_INSERT for TempUsers
            migrationBuilder.Sql("SET IDENTITY_INSERT TempUsers ON");

            // Copy data from the old Users table to the new TempUsers table
            migrationBuilder.Sql("INSERT INTO TempUsers (Id, Email, Password, IsAdmin, Firstname, Middlename, LastName, Gender, Age, Birthday, ContactNumber, Barangay, City, Country, Province, ProfilePhoto) SELECT Id, Email, Password, IsAdmin, Firstname, Middlename, LastName, Gender, Age, Birthday, ContactNumber, Barangay, City, Country, Province, ProfilePhoto FROM Users");

            // Disable IDENTITY_INSERT for TempUsers
            migrationBuilder.Sql("SET IDENTITY_INSERT TempUsers OFF");

            // Drop the old Users table
            migrationBuilder.DropTable(name: "Users");

            // Rename the TempUsers table to Users
            migrationBuilder.RenameTable(name: "TempUsers", newName: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the migration by dropping the Users table and recreating it with the original order
            migrationBuilder.DropTable(name: "Users");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Middlename = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false),
                    Age = table.Column<int>(nullable: false),
                    Barangay = table.Column<string>(nullable: true),
                    Birthday = table.Column<DateTime>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    ContactNumber = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    Firstname = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    ProfilePhoto = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }
    }
}
