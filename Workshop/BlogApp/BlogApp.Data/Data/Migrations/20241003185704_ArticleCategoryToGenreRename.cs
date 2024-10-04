using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ArticleCategoryToGenreRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Articles",
                newName: "Genre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Genre",
                table: "Articles",
                newName: "Category");
        }
    }
}
