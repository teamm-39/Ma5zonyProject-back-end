using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Remove_OperationStoreProductsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "operationStoreProducts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "operationStoreProducts",
                columns: table => new
                {
                    OperationStoreProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromStoreId = table.Column<int>(type: "int", nullable: true),
                    OperationId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ToStoreId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operationStoreProducts", x => x.OperationStoreProductId);
                    table.ForeignKey(
                        name: "FK_operationStoreProducts_Operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operations",
                        principalColumn: "OperationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_operationStoreProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_operationStoreProducts_Stores_FromStoreId",
                        column: x => x.FromStoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId");
                    table.ForeignKey(
                        name: "FK_operationStoreProducts_Stores_ToStoreId",
                        column: x => x.ToStoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_operationStoreProducts_FromStoreId",
                table: "operationStoreProducts",
                column: "FromStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_operationStoreProducts_OperationId",
                table: "operationStoreProducts",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_operationStoreProducts_ProductId",
                table: "operationStoreProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_operationStoreProducts_ToStoreId",
                table: "operationStoreProducts",
                column: "ToStoreId");
        }
    }
}
