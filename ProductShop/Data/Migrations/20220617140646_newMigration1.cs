using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductShop.Data.Migrations
{
    public partial class newMigration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProductViewModel_ProductViewModel_ProductsId",
                table: "OrderProductViewModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductViewModel",
                table: "ProductViewModel");

            migrationBuilder.RenameTable(
                name: "ProductViewModel",
                newName: "ProductViewModels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductViewModels",
                table: "ProductViewModels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProductViewModel_ProductViewModels_ProductsId",
                table: "OrderProductViewModel",
                column: "ProductsId",
                principalTable: "ProductViewModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProductViewModel_ProductViewModels_ProductsId",
                table: "OrderProductViewModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductViewModels",
                table: "ProductViewModels");

            migrationBuilder.RenameTable(
                name: "ProductViewModels",
                newName: "ProductViewModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductViewModel",
                table: "ProductViewModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProductViewModel_ProductViewModel_ProductsId",
                table: "OrderProductViewModel",
                column: "ProductsId",
                principalTable: "ProductViewModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
