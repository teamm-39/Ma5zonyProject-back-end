using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class createproductLogstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductLogs",
                columns: table => new
                {
                    ProductLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldSellingPrice = table.Column<double>(type: "float", nullable: false),
                    OldPurchasePrice = table.Column<double>(type: "float", nullable: false),
                    OldMinLimit = table.Column<int>(type: "int", nullable: false),
                    NewName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewSellingPrice = table.Column<double>(type: "float", nullable: false),
                    NewPurchasePrice = table.Column<double>(type: "float", nullable: false),
                    NewMinLimit = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LookupOperationTypeId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLogs", x => x.ProductLogId);
                    table.ForeignKey(
                        name: "FK_ProductLogs_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductLogs_LookupOperationTypes_LookupOperationTypeId",
                        column: x => x.LookupOperationTypeId,
                        principalTable: "LookupOperationTypes",
                        principalColumn: "LookupOperationTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductLogs_ApplicationUserId",
                table: "ProductLogs",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLogs_LookupOperationTypeId",
                table: "ProductLogs",
                column: "LookupOperationTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductLogs");
        }
    }
}
