using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ArticlesCategoryAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Articles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Articles");
        }
    }
}
