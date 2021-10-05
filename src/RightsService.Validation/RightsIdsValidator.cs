using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Validation
{
  public class RightsIdsValidator : AbstractValidator<IEnumerable<int>>, IRightsIdsValidator
  {
    private List<int> GetRightIds(
      IRightRepository repository,
      IMemoryCache cache)
    {
      if (repository is null)
      {
        throw new ArgumentNullException(nameof(repository));
      }

      List<int> rights = cache.Get<List<int>>(CacheKeys.RightsIds);

      if (rights == null)
      {
        rights = repository.GetRightsList().Select(r => r.RightId).ToList();
        cache.Set(CacheKeys.RightsIds, rights);
      }

      return rights;
    }

    public RightsIdsValidator(
      IRightRepository repository,
      IMemoryCache cache)
    {
      List<int> rights = GetRightIds(repository, cache);

      RuleFor(rightsIds => rightsIds)
        .NotEmpty()
        .WithMessage("Rights list can not be empty");

      RuleForEach(rightsIds => rightsIds)
        .Must(id => rights.Contains(id))
        .WithMessage("Element: {CollectionIndex} of rights list is not correct.");
    }
  }
}
