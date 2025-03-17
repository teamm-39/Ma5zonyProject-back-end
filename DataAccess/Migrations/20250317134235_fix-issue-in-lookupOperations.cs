using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class fixissueinlookupOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_AspNetUsers_ApplicationUserId",
                table: "Operations");

            migrationBuilder.DropIndex(
                name: "IX_Operations_LookupOperationTypeId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "OperationId",
                table: "LookupOperationTypes");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Operations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_LookupOperationTypeId",
                table: "Operations",
                column: "LookupOperationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_AspNetUsers_ApplicationUserId",
                table: "Operations",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_AspNetUsers_ApplicationUserId",
                table: "Operations");

            migrationBuilder.DropIndex(
                name: "IX_Operations_LookupOperationTypeId",
                table: "Operations");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Operations",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "OperationId",
                table: "LookupOperationTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_LookupOperationTypeId",
                table: "Operations",
                column: "LookupOperationTypeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_AspNetUsers_ApplicationUserId",
                table: "Operations",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
