using System;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(RightsServiceDbContext))]
  [Migration("20221120053200_UpdateRightsLocalizationsValues")]
  public class UpdateRightsLocalizationsValues : Migration
  {
    private void UpdateDataInDbRightsLocalization(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DeleteData(
        table: DbRightLocalization.TableName,
        keyColumn: nameof(DbRightLocalization.RightId),
        keyColumnType: "int",
        keyValues: new object[] { 8, 8, 10, 10 });

      migrationBuilder.InsertData(
        table: DbRightLocalization.TableName,
        columns: new[]
        {
          nameof(DbRightLocalization.Id),
          nameof(DbRightLocalization.RightId),
          nameof(DbRightLocalization.Locale),
          nameof(DbRightLocalization.Name),
          nameof(DbRightLocalization.Description)
        },
        columnTypes: new string[]
        {
          "uniqueidentifier", "int", "string", "nvarchar(max)", "nvarchar(max)"
        },
        values: new object[,]
        {
          {
            Guid.NewGuid(), 8, "ru", "Управление офисами",
            "Право позволяет создавать, изменять и архивировать офисы в системе, а также привязывать и отвязывать сотрудников от офисов"
          },
          {
            Guid.NewGuid(), 8, "en", "Offices management",
            "This right allows you to add, edit and remove offices and attach users"
          },
          {
            Guid.NewGuid(), 10, "ru", "Управление бронированием",
            "Право позволяет отменять любые брони, закрывать и открывать помещения для бронирования, настраивать правила бронирования и время работы помещений"
          },
          {
            Guid.NewGuid(), 10, "en", "Booking management",
            "The right allows you to cancel any reservations, close and open rooms for booking, set up booking rules and opening hours of the premises"
          }
        });
    }
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      UpdateDataInDbRightsLocalization(migrationBuilder);
    }
  }
}
