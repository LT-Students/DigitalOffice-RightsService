using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Validation.Helpers.Interfaces
{
    [AutoInject]
    public interface IRoleRightsCompareHelper
    {
        public bool Compare(List<int> addedRights, IRoleRepository repository);
    }
}
