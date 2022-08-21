using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.EFSupport.Provider;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.RightsService.Data.Provider
{
  [AutoInject(InjectType.Scoped)]
    public interface IDataProvider : IBaseDataProvider
    {
        DbSet<DbRightLocalization> RightsLocalizations { get; set; }
        DbSet<DbRole> Roles { get; set; }
        DbSet<DbRoleLocalization> RolesLocalizations { get; set; }
        DbSet<DbRoleRight> RolesRights { get; set;}
        DbSet<DbUserRole> UsersRoles { get; set; }
    }
}
