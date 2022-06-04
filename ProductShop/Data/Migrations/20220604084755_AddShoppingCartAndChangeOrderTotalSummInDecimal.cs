using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductShop.Data.Migrations
{
    public partial class AddShoppingCartAndChangeOrderTotalSummInDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "ShopingCartId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalSum",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ShopingCarts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    IsDone = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopingCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopingCarts_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ShopingCartId",
                table: "Products",
                column: "ShopingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopingCarts_OrderId",
                table: "ShopingCarts",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ShopingCarts_ShopingCartId",
                table: "Products",
                column: "ShopingCartId",
                principalTable: "ShopingCarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ShopingCarts_ShopingCartId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ShopingCarts");

            migrationBuilder.DropIndex(
                name: "IX_Products_ShopingCartId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ShopingCartId",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "TotalSum",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
