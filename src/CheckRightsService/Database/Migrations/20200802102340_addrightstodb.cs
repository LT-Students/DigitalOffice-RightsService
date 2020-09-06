using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.CheckRightsService.Database.Migrations
{
    public partial class AddRightsToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rights",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, null, "Add/Edit/Remove user" });

            migrationBuilder.InsertData(
                table: "Rights",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 2, null, "Add/Edit/Remove project" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rights",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rights",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
