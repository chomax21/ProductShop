using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductShop.Data.Migrations
{
    public partial class newMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProductViewModel_ProductViewModels_ProductsId",
                table: "OrderProductViewModel");

            migrationBuilder.RenameColumn(
                name: "ProductsId",
                table: "OrderProductViewModel",
                newName: "VMProductsId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProductViewModel_ProductsId",
                table: "OrderProductViewModel",
                newName: "IX_OrderProductViewModel_VMProductsId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProductViewModel_ProductViewModels_VMProductsId",
                table: "OrderProductViewModel",
                column: "VMProductsId",
                principalTable: "ProductViewModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProductViewModel_ProductViewModels_VMProductsId",
                table: "OrderProductViewModel");

            migrationBuilder.RenameColumn(
                name: "VMProductsId",
                table: "OrderProductViewModel",
                newName: "ProductsId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProductViewModel_VMProductsId",
                table: "OrderProductViewModel",
                newName: "IX_OrderProductViewModel_ProductsId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProductViewModel_ProductViewModels_ProductsId",
                table: "OrderProductViewModel",
                column: "ProductsId",
                principalTable: "ProductViewModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
