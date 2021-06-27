using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.Kernel.Database;
using Microsoft.EntityFrameworkCore;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Enums;

namespace LT.DigitalOffice.RightsService.Data.Provider
{
    [AutoInject(InjectType.Scoped)]
    public interface IDataProvider : IBaseDataProvider
    {
        DbSet<DbRight> Rights { get; set; }
        DbSet<DbUserRight> RightUsers { get; set; }

        DbSet<DbRole> Roles { get; set; }
        DbSet<DbUserRole> UserRoles { get; set; }

        DbSet<DbUser> Users { get; set; }
    }
}
