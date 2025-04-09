using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addmessageinstorelogstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OlgCity",
                table: "StoreLogs",
                newName: "OldCity");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "StoreLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "StoreLogs");

            migrationBuilder.RenameColumn(
                name: "OldCity",
                table: "StoreLogs",
                newName: "OlgCity");
        }
    }
}
