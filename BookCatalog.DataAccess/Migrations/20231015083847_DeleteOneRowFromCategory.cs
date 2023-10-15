using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookCatalog.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DeleteOneRowFromCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayOrder", "Name" },
                values: new object[] { 7, 7, "Test2" });
        }
    }
}
