using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addstoreLogstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoreLogs",
                columns: table => new
                {
                    StoreLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplivationUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlgCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LookupOperationTypeId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreLogs", x => x.StoreLogId);
                    table.ForeignKey(
                        name: "FK_StoreLogs_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoreLogs_LookupOperationTypes_LookupOperationTypeId",
                        column: x => x.LookupOperationTypeId,
                        principalTable: "LookupOperationTypes",
                        principalColumn: "LookupOperationTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreLogs_ApplicationUserId",
                table: "StoreLogs",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreLogs_LookupOperationTypeId",
                table: "StoreLogs",
                column: "LookupOperationTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreLogs");
        }
    }
}
