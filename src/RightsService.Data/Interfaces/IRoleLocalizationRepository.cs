using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.RightsService.Data.Interfaces
{
  [AutoInject]
  public interface IRoleLocalizationRepository
  {
    bool DoesNameExist(string locale, string name);
  }
}
