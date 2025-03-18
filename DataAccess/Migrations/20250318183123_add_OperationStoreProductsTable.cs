using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class add_OperationStoreProductsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperationStoreProducts",
                columns: table => new
                {
                    OperationStoreProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    FromStoreId = table.Column<int>(type: "int", nullable: true),
                    ToStoreId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationStoreProducts", x => x.OperationStoreProductId);
                    table.ForeignKey(
                        name: "FK_OperationStoreProducts_Operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operations",
                        principalColumn: "OperationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperationStoreProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperationStoreProducts_Stores_FromStoreId",
                        column: x => x.FromStoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId");
                    table.ForeignKey(
                        name: "FK_OperationStoreProducts_Stores_ToStoreId",
                        column: x => x.ToStoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationStoreProducts_FromStoreId",
                table: "OperationStoreProducts",
                column: "FromStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationStoreProducts_OperationId",
                table: "OperationStoreProducts",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationStoreProducts_ProductId",
                table: "OperationStoreProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationStoreProducts_ToStoreId",
                table: "OperationStoreProducts",
                column: "ToStoreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationStoreProducts");
        }
    }
}
