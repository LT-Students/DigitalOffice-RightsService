﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;

namespace LT.DigitalOffice.RightsService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IRightInfoMapper
  {
    RightInfo Map(DbRightsLocalization value);
  }
}
