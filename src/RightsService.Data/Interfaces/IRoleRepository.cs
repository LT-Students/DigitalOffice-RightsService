using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests.Filters;

namespace LT.DigitalOffice.RightsService.Data.Interfaces
{
  [AutoInject]
  public interface IRoleRepository
  {
    Task<Guid> CreateAsync(DbRole dbRole);

    Task<(DbRole role, List<DbUser> users, List<DbRightsLocalization> rights)> GetAsync(GetRoleFilter filter);

    Task<(List<(DbRole role, List<DbRightsLocalization> rights)>, int totalCount)> FindAsync(FindRolesFilter filter);

    Task<bool> DoesRoleExistAsync(Guid roleId);
  }
}
