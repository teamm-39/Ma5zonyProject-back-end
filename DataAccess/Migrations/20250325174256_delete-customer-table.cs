using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class deletecustomertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Customers_CustomerId",
                table: "Operations");

            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Suppliers_SupplierId",
                table: "Operations");

            migrationBuilder.DropTable(
                name: "Customers");

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

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "Suppliers",
                newName: "CustomerSupplierId");

            migrationBuilder.AddColumn<int>(
                name: "CustomerSupplierId",
                table: "Operations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_CustomerSupplierId",
                table: "Operations",
                column: "CustomerSupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Suppliers_CustomerSupplierId",
                table: "Operations",
                column: "CustomerSupplierId",
                principalTable: "Suppliers",
                principalColumn: "CustomerSupplierId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Suppliers_CustomerSupplierId",
                table: "Operations");

            migrationBuilder.DropIndex(
                name: "IX_Operations_CustomerSupplierId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "CustomerSupplierId",
                table: "Operations");

            migrationBuilder.RenameColumn(
                name: "CustomerSupplierId",
                table: "Suppliers",
                newName: "SupplierId");

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

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

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
    }
}
