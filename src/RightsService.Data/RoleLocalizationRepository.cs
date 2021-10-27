using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using Microsoft.EntityFrameworkCore;

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

    public async Task<bool> DoesNameExistAsync(string locale, string name)
    {
      return await _provider.RolesLocalizations.AnyAsync(rl => rl.IsActive && rl.Locale == locale && rl.Name == name);
    }
  }
}
