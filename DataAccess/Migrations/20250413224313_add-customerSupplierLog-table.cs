using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addcustomerSupplierLogtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerSupplierLogs",
                columns: table => new
                {
                    CustomerSupplierLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LookupCustomerSupplierTypeId = table.Column<int>(type: "int", nullable: false),
                    LookupOperationTypeId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldAge = table.Column<int>(type: "int", nullable: false),
                    OldAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldIsReliable = table.Column<bool>(type: "bit", nullable: false),
                    NewName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewAge = table.Column<int>(type: "int", nullable: false),
                    NewAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewIsReliable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSupplierLogs", x => x.CustomerSupplierLogId);
                    table.ForeignKey(
                        name: "FK_CustomerSupplierLogs_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerSupplierLogs_LookupCustomerSupplierType_LookupCustomerSupplierTypeId",
                        column: x => x.LookupCustomerSupplierTypeId,
                        principalTable: "LookupCustomerSupplierType",
                        principalColumn: "LookupCustomerSupplierTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerSupplierLogs_LookupOperationTypes_LookupOperationTypeId",
                        column: x => x.LookupOperationTypeId,
                        principalTable: "LookupOperationTypes",
                        principalColumn: "LookupOperationTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSupplierLogs_ApplicationUserId",
                table: "CustomerSupplierLogs",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSupplierLogs_LookupCustomerSupplierTypeId",
                table: "CustomerSupplierLogs",
                column: "LookupCustomerSupplierTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSupplierLogs_LookupOperationTypeId",
                table: "CustomerSupplierLogs",
                column: "LookupOperationTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerSupplierLogs");
        }
    }
}
