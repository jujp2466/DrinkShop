using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrinkShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AutoMigration_20250819213519 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Drinks",
                newName: "Stock");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Drinks",
                newName: "Quantity");
        }
    }
}
