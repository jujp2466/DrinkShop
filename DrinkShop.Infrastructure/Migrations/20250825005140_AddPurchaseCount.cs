using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrinkShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchaseCount",
                table: "Drinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseCount",
                table: "Drinks");
        }
    }
}
