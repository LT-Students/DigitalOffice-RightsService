using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(RightsServiceDbContext))]
  [Migration("20220515223000_AddRightsLocalizationsValues")]
  public class AddRightsLocalizationsValues : Migration
  {
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
            "Право позволяет создавать, изменять, удалять компании в системе"
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
            "Право позволяет назначать, удалять роль пользователя в системе"
          },
          {
            Guid.NewGuid(), 12, "en", "Users role management",
            "This right allows you to add and remove users role"
          },
          {
            Guid.NewGuid(), 13, "ru", "Управление Вики",
            "Право позволяет создавать, редактировать и удалять статьи, разделы и подразделы в Вики"
          },
          {
            Guid.NewGuid(), 13, "en", "Wiki management",
            "This right allows to create, edit and delete articles, sections in the Wiki"
          },
        });
    }
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      AddDataToDbRightsLocalization(migrationBuilder);
    }
  }
}
