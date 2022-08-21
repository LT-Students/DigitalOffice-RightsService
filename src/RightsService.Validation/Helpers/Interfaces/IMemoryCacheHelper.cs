using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.RightsService.Validation.Helpers.Interfaces
{
  [AutoInject]
  public interface IMemoryCacheHelper
  {
    Task<List<(Guid roleId, bool isActive, IEnumerable<int> rights)>> GetRoleRightsListAsync();
    Task<List<int>> GetRightIdsAsync();
  }
}
