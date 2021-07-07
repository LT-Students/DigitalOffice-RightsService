using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(RightsServiceDbContext))]
    [Migration("20210625000000_AddRolesTables")]
    public class AddRolesTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbRole.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: DbRoleRight.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    RemovedBy = table.Column<Guid>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    RemovedAt = table.Column<DateTime>(nullable: true),
                    RightId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleRights", x => x.Id);
                });

            migrationBuilder.AddColumn<Guid>(
                name: nameof(DbUser.CreatedBy),
                table: DbUser.TableName,
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.AddColumn<Guid>(
                name: "RemovedBy",
                table: DbUser.TableName,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: nameof(DbUser.CreatedAt),
                table: DbUser.TableName,
                nullable: false,
                defaultValue: DateTime.MinValue);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemovedAt",
                table: DbUser.TableName,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: nameof(DbUser.RoleId),
                table: DbUser.TableName,
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.AddColumn<bool>(
                name: nameof(DbUser.IsActive),
                table: DbUser.TableName,
                nullable: false,
                defaultValue: true);
    }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: DbRole.TableName);
            migrationBuilder.DropTable(name: DbRoleRight.TableName);

            migrationBuilder.DropColumn(
                name: nameof(DbUser.CreatedBy),
                table: DbUser.TableName);

            migrationBuilder.DropColumn(
                name: "RemovedBy",
                table: DbUser.TableName);

            migrationBuilder.DropColumn(
                name: nameof(DbUser.CreatedAt),
                table: DbUser.TableName);

            migrationBuilder.DropColumn(
                name: "RemovedAt",
                table: DbUser.TableName);

            migrationBuilder.DropColumn(
                name: nameof(DbUser.RoleId),
                table: DbUser.TableName);

            migrationBuilder.DropColumn(
                name: nameof(DbUser.IsActive),
                table: DbUser.TableName);
        }
    }
}
