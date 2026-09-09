using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastFood.Migrations
{
    public partial class Rename_ProductOrder_To_ProductVariantOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ProductOrder",
                newName: "ProductVariantOrder");

            migrationBuilder.RenameColumn(
                name: "Product_FK",
                table: "ProductVariantOrder",
                newName: "ProductVariant_FK");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductOrder_ProductOrder",
                table: "ProductVariantOrder");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantOrder_ProductVariant_ProductVariant_FK",
                table: "ProductVariantOrder",
                column: "ProductVariant_FK",
                principalTable: "ProductVariant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantOrder_ProductVariant_ProductVariant_FK",
                table: "ProductVariantOrder");

            migrationBuilder.RenameTable(
                name: "ProductVariantOrder",
                newName: "ProductOrder");

            migrationBuilder.RenameColumn(
                name: "ProductVariant_FK",
                table: "ProductOrder",
                newName: "Product_FK");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOrder_ProductOrder",
                table: "ProductOrder",
                column: "Product_FK",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
