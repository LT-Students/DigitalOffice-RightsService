using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.RightsService.Validation.Helpers.Interfaces
{
  [AutoInject]
  public interface ICheckRightsUniquenessHelper
  {
    Task<bool> IsRightsSetUniqueAsync(IEnumerable<int> rightsIds);
  }
}
