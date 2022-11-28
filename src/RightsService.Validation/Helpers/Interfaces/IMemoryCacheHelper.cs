using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Enums;

namespace LT.DigitalOffice.RightsService.Validation.Helpers.Interfaces
{
  [AutoInject(InjectType.Singletone)]
  public interface IMemoryCacheHelper
  {
    Task<List<(Guid roleId, bool isActive, IEnumerable<int> rights)>> GetRoleRightsListAsync();
    Task<List<int>> GetRightIdsAsync();
    Task<List<(Guid userId, Guid roleId)>> GetUsersRolesAsync();
    void Clear(params string[] keys);
  }
}
