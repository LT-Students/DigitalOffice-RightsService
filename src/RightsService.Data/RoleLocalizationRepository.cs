using System.Linq;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;

namespace LT.DigitalOffice.RightsService.Data
{
  public class RoleLocalizationRepository : IRoleLocalizationRepository
  {
    private readonly IDataProvider _provider;

    public RoleLocalizationRepository(
      IDataProvider provider)
    {
      _provider = provider;
    }

    public bool DoesNameExist(string locale, string name)
    {
      return _provider.RolesLocalizations.Any(rl => rl.IsActive && rl.Locale == locale && rl.Name == name);
    }
  }
}
