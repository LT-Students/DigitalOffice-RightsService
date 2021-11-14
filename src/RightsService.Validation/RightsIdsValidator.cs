using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Validation
{
  public class RightsIdsValidator : AbstractValidator<IEnumerable<int>>, IRightsIdsValidator
  {
    private readonly IRightLocalizationRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly IRoleRepository _roleRepository;

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

    private async Task<List<(Guid, IEnumerable<int>)>> GetRightsListAsync()
    {
      List<(Guid roleId, IEnumerable<int> rights)> rights = _cache.Get<List<(Guid, IEnumerable<int>)>>(CacheKeys.RolesRights);

      if (rights == null)
      {
        List<DbRole> roles = await _roleRepository.GetAllWithRightsAsync();

        rights = roles.Select(x => (x.Id, x.RoleRights.Select(x => x.RightId))).ToList();
        _cache.Set(CacheKeys.RolesRights, rights);
      }

      return rights;
    }

    private async Task<bool> CheckRightsUniquenessAsync(IEnumerable<int> rightsIds)
    {
      HashSet<int> addedRights = new(rightsIds);

      IEnumerable<(Guid roleId, IEnumerable<int> rights)> rolesRights = await GetRightsListAsync();

      foreach ((Guid roleId, IEnumerable<int> rights) roleRights in rolesRights)
      {
        if (addedRights.SetEquals(roleRights.rights))
        {
          return false;
        }
      }

      return true;
    }

    public RightsIdsValidator(
      IRightLocalizationRepository repository,
      IMemoryCache cache,
      IRoleRepository roleRepository)
    {
      _repository = repository;
      _cache = cache;
      _roleRepository = roleRepository;

      RuleFor(rightsIds => rightsIds)
        .NotEmpty()
        .WithMessage("Rights list can not be empty")
        .MustAsync(async (rightIds, _) => await CheckRightsUniquenessAsync(rightIds))
        .WithMessage("Set of rights must be unique.");

      RuleForEach(rightsIds => rightsIds)
        .MustAsync(async (id, _) => (await GetRightIdsAsync()).Contains(id))
        .WithMessage("Element: {CollectionIndex} of rights list is not correct.");
    }
  }
}
