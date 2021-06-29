using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.RightsService.Validation.Helpers
{
    public static class CheckRightsHelper
    {
        public static bool DoesExist(IEnumerable<int> rightsIds, IMemoryCache cache, IRightRepository repository)
        {
            var dbRights = new List<DbRight>();

            if (rightsIds == null)
            {
                return true;
            }

            foreach (int rightId in rightsIds)
            {
                if (!cache.TryGetValue(rightId, out object right))
                {
                    if (!dbRights.Any())
                    {
                        dbRights = repository.GetRightsList();
                    }

                    var dbRight = dbRights.Find(right => right.Id == rightId);

                    if (dbRight == null)
                    {
                        return false;
                    }

                    cache.Set(rightId, dbRight);
                }
            }

            return true;
        }
    }
}
