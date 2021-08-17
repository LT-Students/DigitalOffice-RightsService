using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Validation.Helpers.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Validation.Helpers
{
    public class RoleRightsCompareHelper : IRoleRightsCompareHelper
    {
        private readonly IRoleRepository _repository;

        public RoleRightsCompareHelper(IRoleRepository repository)
        {
            _repository = repository;
        }

        public bool Compare(List<int> addedRights)
        {
            IEnumerable<DbRole> roles = _repository.GetAll();
            HashSet<int> hashSet = new(addedRights);

            foreach(DbRole role in roles)
            {
                IEnumerable<int> rights = role.Rights.Select(x => x.RightId);

                if (hashSet.SetEquals(rights))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
