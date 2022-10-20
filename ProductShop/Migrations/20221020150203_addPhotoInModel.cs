using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductShop.Migrations
{
    public partial class addPhotoInModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "ProductViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "ProductViewModels");

            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Products");
        }
    }
}
