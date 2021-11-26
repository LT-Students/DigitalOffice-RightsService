using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.RightsService.Data
{
  /// <inheritdoc cref="IRightLocalizationRepository"/>
  public class RightLocalizationRepository : IRightLocalizationRepository
  {
    private readonly IDataProvider _provider;

    public RightLocalizationRepository(
      IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task<List<DbRightsLocalization>> GetRightsListAsync()
    {
      return await _provider.RightsLocalizations.ToListAsync();
    }

    public async Task<List<DbRightsLocalization>> GetRightsListAsync(string locale)
    {
      return await _provider.RightsLocalizations.Where(r => r.Locale == locale).OrderBy(r => r.RightId).ToListAsync();
    }
  }
}
