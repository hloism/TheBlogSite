using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBlogSite.Migrations
{
    /// <inheritdoc />
    public partial class _002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IDeleted",
                table: "BlogPosts",
                newName: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "BlogPosts",
                newName: "IDeleted");
        }
    }
}
