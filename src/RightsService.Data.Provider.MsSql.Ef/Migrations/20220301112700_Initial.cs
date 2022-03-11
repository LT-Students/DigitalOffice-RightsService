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
          RoleId = table.Column<Guid>(nullable: true),
          IsActive = table.Column<bool>(nullable: false),
          ModifiedBy = table.Column<Guid>(nullable: true),
          ModifiedAtUtc = table.Column<DateTime>(nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PR_Users", x => x.Id);
        });
    }

    private void CreateUsersRightsTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbUserRight.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          RightId = table.Column<int>(nullable: false),
          UserId = table.Column<Guid>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PR_UsersRights", x => x.Id);
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
          table.PrimaryKey("PK_RolesLocalizations", x => x.Id);
        });
    }

    private void AddDataToDbRightsLocalization(MigrationBuilder migrationBuilder)
    {
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
            Guid.NewGuid(), 1, "ru", "Управление пользователями",
            "Право позволяет добавлять, изменять и удалять пользователя из системы"
          },
          {
            Guid.NewGuid(), 1, "en", "User management",
            "This right allows you to add, modify and remove a user from the system"
          },
          {
            Guid.NewGuid(), 2, "ru", "Управление проектами",
            "Право позволяет добавлять, изменять и удалять проекты из системы"
          },
          {
            Guid.NewGuid(), 2,"en", "Project management",
            "This right allows you to add, modify and remove projects from the system"
          },
          {
            Guid.NewGuid(), 3, "ru", "Управление шаблонами сообщений электронной почты",
            "Право позволяет добавлять, изменять и удалять шаблоны сообщений электронной почты " +
            "и оповещений внутри портала из системы"
          },
          {
            Guid.NewGuid(), 3, "en", "Email templates management",
            "This right allows you to add, modify and remove Email templates " +
            "and notification templates from the system"
          },
          {
            Guid.NewGuid(), 4, "ru", "Управление департаментами",
            "Право позволяет добавлять, изменять и удалять департаменты из системы"
          },
          {
            Guid.NewGuid(), 4, "en", "Department management",
            "This right allows you to add, modify and remove departments from the system"
          },
          {
            Guid.NewGuid(), 5, "ru", "Управление новостями",
            "Право позволяет добавлять, изменять и удалять новости из системы"
          },
          {
            Guid.NewGuid(), 5, "en", "News management",
            "This right allows you to add, modify and remove news from the system"
          },
          {
            Guid.NewGuid(), 6, "ru", "Управление должностями, позициями сотрудников",
            "Право позволяет добавлять, изменять и удалять должности, позиции сотрудников из системы"
          },
          {
            Guid.NewGuid(), 6, "en", "Position management",
            "This right allows you to add, modify and remove user positions from the system"
          },
          {
            Guid.NewGuid(), 7, "ru", "Изменение отработанного времени в системе учета времени",
            "Право позволяет добавлять, изменять и удалять отработанное время в системе учета времени"
          },
          {
            Guid.NewGuid(), 7, "en", "Changing of worked hours in the time tracking system",
            "This right allows you to add, modify and remove worked hours in the time tracking system"
          },
          {
            Guid.NewGuid(), 8, "ru", "Управление чейнджлогами спецификаций к API",
            "Право позволяет добавлять, изменять и удалять чейнджлог спецификации к API в системе"
          },
          {
            Guid.NewGuid(), 8, "en", "Changelog of API specification management",
            "This right allows you to add, modify and remove API specification changelog the time system"
          },
          {
            Guid.NewGuid(), 9, "ru", "Управление компаниями",
            "Право позволяет создавать, редактировать, удалять компании в системе"
          },
          {
            Guid.NewGuid(), 9, "en", "company management",
            "This right allows you to add, modify and remove companies  in the system"
          },
          {
            Guid.NewGuid(), 10, "ru", "Управление данными департамента",
            "Право позволяет добавлять и удалять данные департамента к которому привязан правообладатель"
          },
          {
            Guid.NewGuid(), 10, "en", "Department data management",
            "This right allows you to add and remove department data in the department this right holder is attached to"
          },
          {
            Guid.NewGuid(), 11, "ru", "Управление данными компании",
            "Право позволяет создавать, редактировать, изменять данные в компании к которой привязан правообладатель"
          },
          {
            Guid.NewGuid(), 11, "en", "Company data management",
            "This right allows you to add, modify and remove company data in the company this right holder is attached to"
          },
          {
            Guid.NewGuid(), 12, "ru", "Управление ролями пользователей",
            "Право позволяет назначать, изменять и удалять роль пользователя в системе"
          },
          {
            Guid.NewGuid(), 12, "en", "Users role management",
            "This right allows you to add, modify and remove users role"
          }
        });
    }
    #endregion

    protected override void Up(MigrationBuilder migrationBuilder)
    {
      CreateUsersTable(migrationBuilder);

      CreateUsersRightsTable(migrationBuilder);

      CreateRolesTable(migrationBuilder);

      CreateRolesRightsTable(migrationBuilder);

      CreateRightsLocalizationsTable(migrationBuilder);
      AddDataToDbRightsLocalization(migrationBuilder);

      CreateRolesLocalizationsTable(migrationBuilder);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(name: DbRoleRight.TableName);
      migrationBuilder.DropTable(name: DbRightLocalization.TableName);
      migrationBuilder.DropTable(name: DbRoleLocalization.TableName);
      migrationBuilder.DropTable(name: DbUserRight.TableName);
      migrationBuilder.DropTable(name: DbUserRole.TableName);
      migrationBuilder.DropTable(name: DbRole.TableName);
    }  
  }
}
