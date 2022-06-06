using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductShop.Data.Migrations
{
    public partial class CorrectShopingCart3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ShopingCarts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ShopingCarts");
        }
    }
}
