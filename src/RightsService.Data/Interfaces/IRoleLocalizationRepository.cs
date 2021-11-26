using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.RightsService.Data.Interfaces
{
  [AutoInject]
  public interface IRoleLocalizationRepository
  {
    Task<bool> DoesNameExistAsync(string locale, string name);
  }
}
