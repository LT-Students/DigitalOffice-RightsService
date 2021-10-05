using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests.Filters;

namespace LT.DigitalOffice.RightsService.Data.Interfaces
{
  [AutoInject]
  public interface IRoleRepository
  {
    Guid Create(DbRole dbRole);

    (DbRole role, List<DbUser> users, List<DbRightsLocalization> rights) Get(GetRoleFilter filter);

    List<(DbRole role, List<DbRightsLocalization> rights)> Find(FindRolesFilter filter, out int totalCount);

    bool DoesRoleExist(Guid roleId);
  }
}
