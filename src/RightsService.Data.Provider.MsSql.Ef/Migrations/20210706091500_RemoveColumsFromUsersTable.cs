using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(RightsServiceDbContext))]
    [Migration("20210706091500_RemoveColumsFromUsersTable")]
    public class RemoveColumsFromUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemovedBy",
                table: DbUser.TableName);

            migrationBuilder.DropColumn(
                name: "RemovedAt",
                table: DbUser.TableName);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: DbUser.TableName,
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid?>(
                name: "RemovedBy",
                table: DbUser.TableName,
                nullable: true);

            migrationBuilder.AddColumn<DateTime?>(
                name: "RemovedAt",
                table: DbUser.TableName,
                nullable: true);

            migrationBuilder.DropColumn(
                name: "UserId",
                table: DbUser.TableName);
        }
    }
}
