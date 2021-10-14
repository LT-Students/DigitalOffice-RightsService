using System;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(RightsServiceDbContext))]
  [Migration("20211001085800_ReworkRightTable")]
  public class ReworkRightTable : Migration
  {
    private void CreateRecord(
        MigrationBuilder migrationBuilder,
        int rightId,
        string locale,
        string name,
        string description)
    {
      migrationBuilder.InsertData
      (
          table: DbRightsLocalization.TableName,
          columns: new[]
          {
            nameof(DbRightsLocalization.Id),
            nameof(DbRightsLocalization.RightId),
            nameof(DbRightsLocalization.Locale),
            nameof(DbRightsLocalization.Name),
            nameof(DbRightsLocalization.Description)
          },
          columnTypes: new string[]
          {
            "uniqueidentifier",
            "int",
            "string",
            "nvarchar(max)",
            "nvarchar(max)"
          },
          values: new object[]
          {
            Guid.NewGuid(),
            rightId,
            locale,
            name,
            description
          }
      );
    }

    private void AddRights(MigrationBuilder migrationBuilder)
    {
      CreateRecord(migrationBuilder, 1, "ru", "Управление пользователями",
        "Право позволяет добавлять, изменять и удалять пользователя из системы");
      CreateRecord(migrationBuilder, 1, "en", "User management",
        "This right allows you to add, modify and remove a user from the system");

      CreateRecord(migrationBuilder, 2, "ru", "Управление проектами",
        "Право позволяет добавлять, изменять и удалять проекты из системы");
      CreateRecord(migrationBuilder, 2, "en", "Project management",
        "This right allows you to add, modify and remove projects from the system");

      CreateRecord(migrationBuilder, 3, "ru", "Управление шаблонами сообщений электронной почты",
        "Право позволяет добавлять, изменять и удалять шаблоны сообщений электронной почты из системы");
      CreateRecord(migrationBuilder, 3, "en", "Email templates management",
        "This right allows you to add, modify and remove Email templates from the system");

      CreateRecord(migrationBuilder, 4, "ru", "Управление департаментами",
        "Право позволяет добавлять, изменять и удалять департаменты из системы");
      CreateRecord(migrationBuilder, 4, "en", "Department management",
        "This right allows you to add, modify and remove departments from the system");

      CreateRecord(migrationBuilder, 5, "ru", "Управление новостями",
        "Право позволяет добавлять, изменять и удалять новости из системы");
      CreateRecord(migrationBuilder, 5, "en", "News management",
        "This right allows you to add, modify and remove news from the system");

      CreateRecord(migrationBuilder, 6, "ru", "Управление должностями, позициями сотрудников",
        "Право позволяет добавлять, изменять и удалять должности, позиции сотрудников из системы");
      CreateRecord(migrationBuilder, 6, "en", "Position management",
        "This right allows you to add, modify and remove user positions from the system");

      CreateRecord(migrationBuilder, 7, "ru", "Изменение отработанного времени в системе учета времени",
        "Право позволяет добавлять, изменять и удалять отработанное время в системе учета времени");
      CreateRecord(migrationBuilder, 7, "en", "Changing of worked hours in the time tracking system",
        "This right allows you to add, modify and remove worked hours in the time tracking system");

      CreateRecord(migrationBuilder, 8, "ru", "Управление чейнджлогами спецификаций к API ",
        "Право позволяет добавлять, изменять и удалять чейнджлог спецификации к API в системе");
      CreateRecord(migrationBuilder, 8, "en", "Changelog of API specification management",
        "This right allows you to add, modify and remove API specification changelog the time system");

      CreateRecord(migrationBuilder, 9, "ru", "Управление структурой компании",
        "Право позволяет изменять структуру компании в системе");
      CreateRecord(migrationBuilder, 9, "en", "Company restructuring management",
        "This right allows you to modify company structure in the system");

      CreateRecord(migrationBuilder, 10, "ru", "Управление сотрудниками департамента",
        "Право позволяет добавлять, изменять и удалять сотрудника департамента в системе");
      CreateRecord(migrationBuilder, 10, "en", "Department user management",
        "This right allows you to add, modify and remove department user in the time system");
    }

    private void CreateRightsLocalizationsTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbRightsLocalization.TableName,
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
          table.PrimaryKey("PK_RightsLocalization", x => x.Id);
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
          table.PrimaryKey("PK_RoleLocalization", x => x.Id);
        });
    }

    private void UpdateRolesTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropUniqueConstraint(
        name: "UC_Roles_Name",
        table: DbRole.TableName);

      migrationBuilder.RenameColumn(
        name: "CreatedAt",
        newName: "CreatedAtUtc",
        table: DbRole.TableName);

      migrationBuilder.AddColumn<Guid>(
        name: "ModifiedBy",
        table: DbRole.TableName,
        nullable: true);

      migrationBuilder.AddColumn<DateTime>(
        name: "ModifiedAtUtc",
        table: DbRole.TableName,
        nullable: true);

      migrationBuilder.DropColumn(
        name: "Name",
        table: DbRole.TableName);

      migrationBuilder.DropColumn(
        name: "Description",
        table: DbRole.TableName);
    }

    private void UpdateRoleRightsTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.RenameColumn(
        name: "CreatedAt",
        newName: "CreatedAtUtc",
        table: DbRoleRight.TableName);

      migrationBuilder.DropColumn(
        name: "RemovedAt",
        table: DbRoleRight.TableName);

      migrationBuilder.DropColumn(
        name: "RemovedBy",
        table: DbRoleRight.TableName);
    }

    private void UpdateUsersTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.RenameColumn(
        name: "CreatedAt",
        newName: "CreatedAtUtc",
        table: DbUser.TableName);

      migrationBuilder.AddColumn<Guid>(
        name: "ModifiedBy",
        table: DbUser.TableName,
        nullable: true);

      migrationBuilder.AddColumn<DateTime>(
        name: "ModifiedAtUtc",
        table: DbUser.TableName,
        nullable: true);
    }

    private void UpdateUsersRightsTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<Guid>(
        name: "CreatedBy",
        table: DbUserRight.TableName,
        nullable: false);

      migrationBuilder.AddColumn<DateTime>(
        name: "CreatedAtUtc",
        table: DbUserRight.TableName,
        nullable: false);
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable("Rights");

      UpdateRolesTable(migrationBuilder);
      CreateRightsLocalizationsTable(migrationBuilder);
      CreateRolesLocalizationsTable(migrationBuilder);
      UpdateRoleRightsTable(migrationBuilder);
      UpdateUsersRightsTable(migrationBuilder);
      UpdateUsersTable(migrationBuilder);

      AddRights(migrationBuilder);
    }
  }
}
