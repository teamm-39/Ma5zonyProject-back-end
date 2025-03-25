using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class renamesuppliertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Suppliers_CustomerSupplierId",
                table: "Operations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers");

            migrationBuilder.RenameTable(
                name: "Suppliers",
                newName: "CustomersSuppliers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomersSuppliers",
                table: "CustomersSuppliers",
                column: "CustomerSupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_CustomersSuppliers_CustomerSupplierId",
                table: "Operations",
                column: "CustomerSupplierId",
                principalTable: "CustomersSuppliers",
                principalColumn: "CustomerSupplierId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_CustomersSuppliers_CustomerSupplierId",
                table: "Operations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomersSuppliers",
                table: "CustomersSuppliers");

            migrationBuilder.RenameTable(
                name: "CustomersSuppliers",
                newName: "Suppliers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers",
                column: "CustomerSupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Suppliers_CustomerSupplierId",
                table: "Operations",
                column: "CustomerSupplierId",
                principalTable: "Suppliers",
                principalColumn: "CustomerSupplierId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
