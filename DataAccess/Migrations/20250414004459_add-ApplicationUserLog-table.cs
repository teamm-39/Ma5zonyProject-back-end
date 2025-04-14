using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addApplicationUserLogtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUserLogs",
                columns: table => new
                {
                    ApplicationUserLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldAge = table.Column<int>(type: "int", nullable: false),
                    OldUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewAge = table.Column<int>(type: "int", nullable: false),
                    NewUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserLogs", x => x.ApplicationUserLogId);
                    table.ForeignKey(
                        name: "FK_ApplicationUserLogs_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserLogs_ApplicationUserId",
                table: "ApplicationUserLogs",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserLogs");
        }
    }
}
