using LT.DigitalOffice.CheckRightsService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.CheckRightsService.Data.Provider
{
    public interface IDataProvider
    {
        DbSet<DbRight> Rights { get; set; }
        DbSet<DbRightUser> RightUsers { get; set; }

        void SaveChanges();
        object MakeEntityDetached(object obj);
        void EnsureDeleted();
        bool IsInMemory();
    }
}
