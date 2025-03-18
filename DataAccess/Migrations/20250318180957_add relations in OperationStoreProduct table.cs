using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addrelationsinOperationStoreProducttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_operationStoreProducts_Stores_StoreId",
                table: "operationStoreProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_operationStoreProducts",
                table: "operationStoreProducts");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                table: "operationStoreProducts",
                newName: "OperationStoreProductId");

            migrationBuilder.AddColumn<int>(
                name: "FromStoreId",
                table: "operationStoreProducts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToStoreId",
                table: "operationStoreProducts",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_operationStoreProducts",
                table: "operationStoreProducts",
                column: "OperationStoreProductId");

            migrationBuilder.CreateIndex(
                name: "IX_operationStoreProducts_FromStoreId",
                table: "operationStoreProducts",
                column: "FromStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_operationStoreProducts_ToStoreId",
                table: "operationStoreProducts",
                column: "ToStoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_operationStoreProducts_Stores_FromStoreId",
                table: "operationStoreProducts",
                column: "FromStoreId",
                principalTable: "Stores",
                principalColumn: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_operationStoreProducts_Stores_ToStoreId",
                table: "operationStoreProducts",
                column: "ToStoreId",
                principalTable: "Stores",
                principalColumn: "StoreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_operationStoreProducts_Stores_FromStoreId",
                table: "operationStoreProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_operationStoreProducts_Stores_ToStoreId",
                table: "operationStoreProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_operationStoreProducts",
                table: "operationStoreProducts");

            migrationBuilder.DropIndex(
                name: "IX_operationStoreProducts_FromStoreId",
                table: "operationStoreProducts");

            migrationBuilder.DropIndex(
                name: "IX_operationStoreProducts_ToStoreId",
                table: "operationStoreProducts");

            migrationBuilder.DropColumn(
                name: "FromStoreId",
                table: "operationStoreProducts");

            migrationBuilder.DropColumn(
                name: "ToStoreId",
                table: "operationStoreProducts");

            migrationBuilder.RenameColumn(
                name: "OperationStoreProductId",
                table: "operationStoreProducts",
                newName: "StoreId");


            migrationBuilder.AddPrimaryKey(
                name: "PK_operationStoreProducts",
                table: "operationStoreProducts",
                columns: new[] { "StoreId", "ProductId", "OperationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_operationStoreProducts_Stores_StoreId",
                table: "operationStoreProducts",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
