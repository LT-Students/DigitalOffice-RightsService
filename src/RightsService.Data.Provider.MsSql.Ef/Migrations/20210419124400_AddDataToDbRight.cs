using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(RightsServiceDbContext))]
    [Migration("20210419124400_AddDataToDbRight")]
    public class AddDataToDbRight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rights",
                columns: new[] { "Id", "Name", "Description" },
                columnTypes: new string[]
                {
                    "int",
                    "nvarchar(max)",
                    "nvarchar(max)"
                },
                values: new object[] { 5, "Add/Edit/Remove news", null });

            migrationBuilder.InsertData(
                table: "Rights",
                columns: new[] { "Id", "Name", "Description" },
                columnTypes: new string[]
                {
                    "int",
                    "nvarchar(max)",
                    "nvarchar(max)"
                },
                values: new object[] { 6, "Add/Edit/Remove positions", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rights",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Rights",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
