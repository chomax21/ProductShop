using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductShop.Migrations
{
    public partial class _3_Migration_fixKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OriginProductId",
                table: "ProductViewModels",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginProductId",
                table: "ProductViewModels");
        }
    }
}
