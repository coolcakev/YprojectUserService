using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YprojectUserService.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameForCategoryTitleId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoryTitleId",
                table: "UserCategories",
                newName: "CategoryTitleKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoryTitleKey",
                table: "UserCategories",
                newName: "CategoryTitleId");
        }
    }
}
