using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Validation
{
  public class RightsIdsValidator : AbstractValidator<IEnumerable<int>>, IRightsIdsValidator
  {
    private readonly IRightLocalizationRepository _repository;
    private readonly IMemoryCache _cache;

    // todo rework
    private async Task<List<int>> GetRightIdsAsync()
    {
      List<int> rights = _cache.Get<List<int>>(CacheKeys.RightsIds);

      if (rights == null)
      {
        rights = (await _repository.GetRightsListAsync()).Select(r => r.RightId).ToList();
        _cache.Set(CacheKeys.RightsIds, rights);
      }

      return rights;
    }

    public RightsIdsValidator(
      IRightLocalizationRepository repository,
      IMemoryCache cache)
    {
      _repository = repository;
      _cache = cache;

      RuleFor(rightsIds => rightsIds)
        .NotEmpty()
        .WithMessage("Rights list can not be empty");

      RuleForEach(rightsIds => rightsIds)
        .MustAsync(async (id, _) => (await GetRightIdsAsync()).Contains(id))
        .WithMessage("Element: {CollectionIndex} of rights list is not correct.");
    }
  }
}
