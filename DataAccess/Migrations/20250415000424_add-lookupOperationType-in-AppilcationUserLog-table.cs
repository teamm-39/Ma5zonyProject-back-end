using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addlookupOperationTypeinAppilcationUserLogtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LookupOperationTypeId",
                table: "ApplicationUserLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserLogs_LookupOperationTypeId",
                table: "ApplicationUserLogs",
                column: "LookupOperationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserLogs_LookupOperationTypes_LookupOperationTypeId",
                table: "ApplicationUserLogs",
                column: "LookupOperationTypeId",
                principalTable: "LookupOperationTypes",
                principalColumn: "LookupOperationTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserLogs_LookupOperationTypes_LookupOperationTypeId",
                table: "ApplicationUserLogs");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserLogs_LookupOperationTypeId",
                table: "ApplicationUserLogs");

            migrationBuilder.DropColumn(
                name: "LookupOperationTypeId",
                table: "ApplicationUserLogs");
        }
    }
}
