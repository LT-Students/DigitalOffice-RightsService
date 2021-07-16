using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(RightsServiceDbContext))]
    [Migration("20210403032900_Refactoring")]
    public class _20210403032900_Refactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("RightUsers");
            migrationBuilder.DropTable("DbRightUser");

            migrationBuilder.CreateTable(
                name: DbUserRight.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RightId = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PR_UserRights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: DbUser.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PR_Users", x => x.Id);
                });
        }
    }
}
