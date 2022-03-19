using System;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(RightsServiceDbContext))]
  [Migration("20220301112700_Initial")]
  public class Initial : Migration
  {
    #region private methods
    private void CreateUsersTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbUserRole.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          UserId = table.Column<Guid>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          RoleId = table.Column<Guid>(nullable: false),
          IsActive = table.Column<bool>(nullable: false),
          ModifiedBy = table.Column<Guid>(nullable: true),
          ModifiedAtUtc = table.Column<DateTime>(nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PR_UsersRoles", x => x.Id);
        });
    }

    private void CreateRolesTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbRole.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          IsActive = table.Column<bool>(nullable: false),
          ModifiedBy = table.Column<Guid>(nullable: true),
          ModifiedAtUtc = table.Column<DateTime>(nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Roles", x => x.Id);
        });
    }

    private void CreateRolesRightsTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbRoleRight.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          RoleId = table.Column<Guid>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          RightId = table.Column<int>(nullable: false),
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_RolesRights", x => x.Id);
        });
    }

    private void CreateRightsLocalizationsTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbRightLocalization.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          RightId = table.Column<int>(nullable: false),
          Locale = table.Column<string>(nullable: false, maxLength: 2),
          Name = table.Column<string>(nullable: false),
          Description = table.Column<string>(nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_RightsLocalizations", x => x.Id);
        });
    }

    private void CreateRolesLocalizationsTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbRoleLocalization.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          RoleId = table.Column<Guid>(nullable: false),
          Locale = table.Column<string>(nullable: false, maxLength: 2),
          Name = table.Column<string>(nullable: false),
          Description = table.Column<string>(nullable: true),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          ModifiedBy = table.Column<Guid>(nullable: true),
          ModifiedAtUtc = table.Column<DateTime>(nullable: true),
          IsActive = table.Column<bool>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_RolesLocalizations", x => x.Id);
        });
    }
    #endregion

    protected override void Up(MigrationBuilder migrationBuilder)
    {
      CreateUsersTable(migrationBuilder);

      CreateRolesTable(migrationBuilder);

      CreateRolesRightsTable(migrationBuilder);

      CreateRightsLocalizationsTable(migrationBuilder);

      CreateRolesLocalizationsTable(migrationBuilder);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(name: DbRoleRight.TableName);
      migrationBuilder.DropTable(name: DbRightLocalization.TableName);
      migrationBuilder.DropTable(name: DbRoleLocalization.TableName);
      migrationBuilder.DropTable(name: DbUserRole.TableName);
      migrationBuilder.DropTable(name: DbRole.TableName);
    }  
  }
}
