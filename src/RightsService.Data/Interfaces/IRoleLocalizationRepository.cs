﻿using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.RightsService.Data.Interfaces
{
  [AutoInject]
  public interface IRoleLocalizationRepository
  {
    Task<Guid?> CreateAsync(DbRoleLocalization roleLocalization);
    Task<DbRoleLocalization> GetAsync(Guid roleLocalizationId);
    Task<bool> DoesLocaleExistAsync(Guid roleId, string locale);
    Task<bool> DoesNameExistAsync(string locale, string name, Guid id = default);
    Task<bool> EditRoleLocalizationAsync(Guid roleLocalizationId, JsonPatchDocument<DbRoleLocalization> patch);
  }
}
