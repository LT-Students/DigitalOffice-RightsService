using System;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(RightsServiceDbContext))]
  [Migration("20211011091700_UpdateUserTable")]
  public class UpdateUserTable : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AlterColumn<Guid>(
        name: "RoleId",
        table: DbUser.TableName,
        nullable: true);
    }
  }
}
