using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(RightsServiceDbContext))]
    [Migration("20210419124400_AddDataToDbRight")]
    public class _20210427124911_AddDataToDbRight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: DbRight.TableName,
                columns: new[] { nameof(DbRight.Id), nameof(DbRight.Name), nameof(DbRight.Description) },
                columnTypes: new string[]
                {
                    "int",
                    "nvarchar(max)",
                    "nvarchar(max)"
                },
                values: new object[] { 7, "Add/Edit/Remove tasks", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: DbRight.TableName,
                keyColumn: nameof(DbRight.Id),
                keyValue: 7);
        }
    }
}