using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef.Migrations
{
  [Migration("20220120201000_UpdateRightsLocalizationTable")]
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
        values: new[] { "Удаление и добавление сотрудников департамента", "Право позволяет удалять и добавлять сотрудника департамента в системе" });

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
        values: new[] { "Removing and adding department users", "This right allows you to add and remove department user in the time system" });
    }
  }
}
