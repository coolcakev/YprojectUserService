using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YprojectUserService.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAgeGroupForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgeGroup",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeGroup",
                table: "Users");
        }
    }
}
