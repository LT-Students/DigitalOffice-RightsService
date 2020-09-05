using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.CheckRightsService.Database.Migrations
{
    public partial class AddRightDbUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DbRightUser",
                columns: table => new
                {
                    RightId = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbRightUser", x => new { x.RightId, x.UserId });
                    table.ForeignKey(
                        name: "FK_DbRightUser_Rights_RightId",
                        column: x => x.RightId,
                        principalTable: "Rights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbRightUser");
        }
    }
}
