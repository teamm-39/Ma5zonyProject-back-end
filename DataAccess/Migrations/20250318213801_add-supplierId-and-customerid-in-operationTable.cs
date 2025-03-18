using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addsupplierIdandcustomeridinoperationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Operations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "Operations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_CustomerId",
                table: "Operations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_SupplierId",
                table: "Operations",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Customers_CustomerId",
                table: "Operations",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Suppliers_SupplierId",
                table: "Operations",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Customers_CustomerId",
                table: "Operations");

            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Suppliers_SupplierId",
                table: "Operations");

            migrationBuilder.DropIndex(
                name: "IX_Operations_CustomerId",
                table: "Operations");

            migrationBuilder.DropIndex(
                name: "IX_Operations_SupplierId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Operations");
        }
    }
}
