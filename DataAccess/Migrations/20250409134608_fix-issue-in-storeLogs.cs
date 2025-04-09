using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class fixissueinstoreLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreLogs_AspNetUsers_ApplicationUserId",
                table: "StoreLogs");

            migrationBuilder.DropColumn(
                name: "ApplivationUserId",
                table: "StoreLogs");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "StoreLogs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreLogs_AspNetUsers_ApplicationUserId",
                table: "StoreLogs",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreLogs_AspNetUsers_ApplicationUserId",
                table: "StoreLogs");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "StoreLogs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplivationUserId",
                table: "StoreLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreLogs_AspNetUsers_ApplicationUserId",
                table: "StoreLogs",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
