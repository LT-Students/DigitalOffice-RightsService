using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.Kernel.Database;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.RightsService.Data.Provider
{
    public interface IDataProvider : IBaseDataProvider
    {
        DbSet<DbRight> Rights { get; set; }
        DbSet<DbUserRight> RightUsers { get; set; }
        DbSet<DbUser> Users { get; set; }
    }
}
