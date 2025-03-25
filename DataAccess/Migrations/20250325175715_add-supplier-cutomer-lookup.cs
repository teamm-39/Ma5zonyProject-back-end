using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addsuppliercutomerlookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LookupCustomerSupplierTypeId",
                table: "CustomersSuppliers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LookupCustomerSupplierType",
                columns: table => new
                {
                    LookupCustomerSupplierTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupCustomerSupplierType", x => x.LookupCustomerSupplierTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomersSuppliers_LookupCustomerSupplierTypeId",
                table: "CustomersSuppliers",
                column: "LookupCustomerSupplierTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersSuppliers_LookupCustomerSupplierType_LookupCustomerSupplierTypeId",
                table: "CustomersSuppliers",
                column: "LookupCustomerSupplierTypeId",
                principalTable: "LookupCustomerSupplierType",
                principalColumn: "LookupCustomerSupplierTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomersSuppliers_LookupCustomerSupplierType_LookupCustomerSupplierTypeId",
                table: "CustomersSuppliers");

            migrationBuilder.DropTable(
                name: "LookupCustomerSupplierType");

            migrationBuilder.DropIndex(
                name: "IX_CustomersSuppliers_LookupCustomerSupplierTypeId",
                table: "CustomersSuppliers");

            migrationBuilder.DropColumn(
                name: "LookupCustomerSupplierTypeId",
                table: "CustomersSuppliers");
        }
    }
}
