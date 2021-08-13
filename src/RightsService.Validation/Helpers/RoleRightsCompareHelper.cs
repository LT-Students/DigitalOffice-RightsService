using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Validation.Helpers.Interfaces;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Validation.Helpers
{
    public class RoleRightsCompareHelper : IRoleRightsCompareHelper
    {
        public bool Compare(List<int> addedRights, IRoleRepository repository)
        {
            IEnumerable<DbRole> roles = repository.GetAll();
            HashSet<int> hashSet = new(addedRights);

            foreach(DbRole role in roles)
            {
                List<int> rights = new();

                foreach(DbRoleRight right in role.Rights)
                {
                    rights.Add(right.RightId);
                }

                if (hashSet.SetEquals(rights))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
