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

    Task<(DbRole role, List<DbUserRole> users, List<DbRightLocalization> rights)> GetAsync(GetRoleFilter filter);

    Task<DbRole> GetAsync(Guid roleId);

    Task<List<DbRole>> GetAsync(List<Guid> rolesIds);

    Task<List<DbRole>> GetAllWithRightsAsync();

    Task<(List<(DbRole role, List<DbRightLocalization> rights)>, int totalCount)> FindAllAsync(FindRolesFilter filter);

    Task<(List<(DbRole role, List<DbRightLocalization> rights)>, int totalCount)> FindActiveAsync(FindRolesFilter filter);

    Task<bool> DoesRoleExistAsync(Guid roleId);

    Task<bool> EditStatusAsync(Guid roleId, bool isActive);

    Task<bool> EditRoleRightsAsync(Guid roleId, List<DbRoleRight> newRights);
  }
}
