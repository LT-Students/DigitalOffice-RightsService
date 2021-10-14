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
        DbSet<DbRightsLocalization> RightsLocalizations { get; set; }
        DbSet<DbRole> Roles { get; set; }
        DbSet<DbRoleLocalization> RolesLocalizations { get; set; }
        DbSet<DbRoleRight> RoleRights { get; set;}
        DbSet<DbUser> Users { get; set; }
        DbSet<DbUserRight> UserRights { get; set; }
    }
}
