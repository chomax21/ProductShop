using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductShop.Data.Migrations
{
    public partial class CorrectShopingCart2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ShopingCarts_ShopingCartId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ShopingCartId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ShopingCartId",
                table: "Products");

            migrationBuilder.AddColumn<bool>(
                name: "isPayed",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isPayed",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "ShopingCartId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ShopingCartId",
                table: "Products",
                column: "ShopingCartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ShopingCarts_ShopingCartId",
                table: "Products",
                column: "ShopingCartId",
                principalTable: "ShopingCarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
