using LT.DigitalOffice.CheckRightsService.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace TimeManagementService.Data.Provider
{
    public interface IDataProvider
    {
        DbSet<DbRight> Rights { get; set; }

        void SaveChanges();
        object MakeEntityDetached(object obj);
        void EnsureDeleted();
        bool IsInMemory();
    }
}
