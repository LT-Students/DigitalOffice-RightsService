using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef.Migrations
{
  [Migration("20220126211000_UpdateRightsLocalizationTable")]
  [DbContext(typeof(RightsServiceDbContext))]
  public class UpdateRightsLocalizationTable : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder
        .UpdateData(
        table: DbRightsLocalization.TableName,
        keyColumns: new[] { "RightId", "Locale" }, 
        keyColumnTypes: new []
        {
          "int",
          "string",
        },
        keyValues: new object[] { 10, "ru" },
        columns: new[] { "Name", "Description" },
        columnTypes: new[] { "string", "string" },
        values: new[] { "Управление данными департамента", "Право позволяет добавлять, изменять " +
        "и удалять сотрудника департамента в системе, добавлять/удалять проекты и новости в департаменте" });

      migrationBuilder
        .UpdateData(
        table: DbRightsLocalization.TableName,
        keyColumns: new[] { "RightId", "Locale" },
        keyColumnTypes: new[]
        {
          "int",
          "string",
        },
        keyValues: new object[] { 10, "en" },
        columns: new[] { "Name", "Description" },
        columnTypes: new[] { "string", "string" },
        values: new[] { "Department data management", "This right allows you to add, modify " +
        "and remove department user in the time system; to add, modify " +
        "and remove projects and news in the department this right is attached to" });
    }
  }
}
