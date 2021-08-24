using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(RightsServiceDbContext))]
    [Migration("20210824090600_AddUniqueConstraintToRolesTable")]
    public class AddUniqueConstraintToRolesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: DbRole.TableName,
                maxLength: 100);

            migrationBuilder.AddUniqueConstraint(
                name: "UC_Roles_Name",
                table: DbRole.TableName,
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "UC_Roles_Name",
                table: DbRole.TableName);
        }
    }
}
