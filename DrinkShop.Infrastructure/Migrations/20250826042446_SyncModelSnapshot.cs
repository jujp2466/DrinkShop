using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrinkShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_items_orders_OrderId1",
                table: "order_items");

            migrationBuilder.DropIndex(
                name: "IX_order_items_OrderId1",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "order_items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId1",
                table: "order_items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_items_OrderId1",
                table: "order_items",
                column: "OrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_orders_OrderId1",
                table: "order_items",
                column: "OrderId1",
                principalTable: "orders",
                principalColumn: "Id");
        }
    }
}
