using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Validation.Options
{
    public class CacheOptions
    {
        public const string MemoryCache = "MemoryCache";

        public IEnumerable<int> ExistingRights { get; set; }
    }
}
