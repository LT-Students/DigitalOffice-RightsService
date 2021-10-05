﻿using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;

namespace LT.DigitalOffice.RightsService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbRoleLocalizationMapper
  {
    DbRoleLocalization Map(CreateRoleLocalizationRequest request);
    DbRoleLocalization Map(CreateRoleLocalizationRequest request, Guid roleId);
  }
}
