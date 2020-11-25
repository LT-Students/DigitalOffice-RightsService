using LT.DigitalOffice.CheckRightsService.Models.Db;
using LT.DigitalOffice.Kernel.Database;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.CheckRightsService.Data.Provider
{
    public interface IDataProvider : IBaseDataProvider
    {
        DbSet<DbRight> Rights { get; set; }
        DbSet<DbRightUser> RightUsers { get; set; }
    }
}
