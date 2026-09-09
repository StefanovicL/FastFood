using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastFood.Migrations
{
    public partial class CreateProductIngredientRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Product",
                table: "Ingredient");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Stock",
                table: "Ingredient");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProducts_Order_OrderID",
                table: "OrderProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProducts_Product_ProductID",
                table: "OrderProducts");

            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts");

            //migrationBuilder.DropIndex(
            //    name: "IX_Ingredient_ProductID",
            //    table: "Ingredient");

            //migrationBuilder.DropIndex(
            //    name: "IX_Ingredient_StockID",
            //    table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "OrderProductID",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "ProductID",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "StockID",
                table: "Ingredient");

            migrationBuilder.RenameColumn(
                name: "RoleID",
                table: "UserRole",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "UserRole",
                newName: "UserId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_UserRole_RoleID",
            //    table: "UserRole",
            //    newName: "IX_UserRole_RoleId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "User",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "RoleID",
                table: "Role",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "ProductID",
                table: "Product",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "OrderID",
                table: "OrderProducts",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "ProductID",
                table: "OrderProducts",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProducts_OrderID",
                table: "OrderProducts",
                newName: "IX_OrderProducts_OrderId");

            migrationBuilder.RenameColumn(
                name: "OrderID",
                table: "Order",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "IngredientID",
                table: "Ingredient",
                newName: "IngredientId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Ingredient",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts",
                columns: new[] { "ProductId", "OrderId" });

            migrationBuilder.CreateTable(
                name: "ProductIngredient",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    IngredientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductIngredient", x => new { x.ProductId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_ProductIngredient_Ingredient_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredient",
                        principalColumn: "IngredientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductIngredient_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductIngredient_IngredientId",
                table: "ProductIngredient",
                column: "IngredientId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProducts_Order_OrderId",
                table: "OrderProducts",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProducts_Product_ProductId",
                table: "OrderProducts",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProducts_Order_OrderId",
                table: "OrderProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProducts_Product_ProductId",
                table: "OrderProducts");

            migrationBuilder.DropTable(
                name: "ProductIngredient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Ingredient");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "UserRole",
                newName: "RoleID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserRole",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                newName: "IX_UserRole_RoleID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "User",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Role",
                newName: "RoleID");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Product",
                newName: "ProductID");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "OrderProducts",
                newName: "OrderID");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderProducts",
                newName: "ProductID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProducts_OrderId",
                table: "OrderProducts",
                newName: "IX_OrderProducts_OrderID");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Order",
                newName: "OrderID");

            migrationBuilder.RenameColumn(
                name: "IngredientId",
                table: "Ingredient",
                newName: "IngredientID");

            migrationBuilder.AddColumn<int>(
                name: "OrderProductID",
                table: "OrderProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductID",
                table: "Ingredient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StockID",
                table: "Ingredient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts",
                column: "ProductID");

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    StockID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.StockID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_ProductID",
                table: "Ingredient",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_StockID",
                table: "Ingredient",
                column: "StockID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Product",
                table: "Ingredient",
                column: "ProductID",
                principalTable: "Product",
                principalColumn: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Stock",
                table: "Ingredient",
                column: "StockID",
                principalTable: "Stock",
                principalColumn: "StockID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProducts_Order_OrderID",
                table: "OrderProducts",
                column: "OrderID",
                principalTable: "Order",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProducts_Product_ProductID",
                table: "OrderProducts",
                column: "ProductID",
                principalTable: "Product",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
