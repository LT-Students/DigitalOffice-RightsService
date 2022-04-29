using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.RightsService.Data
{
  public class UserRoleRepository : IUserRoleRepository
  {
    private readonly IDataProvider _provider;

    public UserRoleRepository(
      IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task<Guid?> CreateAsync(DbUserRole dbUserRole)
    {
      if (dbUserRole is null)
      {
        return default;
      }

      _provider.UsersRoles.Add(dbUserRole);
      await _provider.SaveAsync();

      return dbUserRole.Id;
    }

    public async Task<bool> CheckRightsAsync(Guid userId, params int[] rightIds)
    {
      if (rightIds == null)
      {
        return false;
      }

      List<int> rights = (await
        (from user in _provider.UsersRoles
         where user.UserId == userId && user.IsActive
         join role in _provider.Roles on user.RoleId equals role.Id
         where role.IsActive
         join roleRight in _provider.RolesRights on role.Id equals roleRight.RoleId
         select new
         {
           Right = roleRight
         }).ToListAsync()).AsEnumerable().GroupBy(r => r)
        .Select(x =>
        {
          return x.Select(x => x.Right.RightId).FirstOrDefault();
        }).ToList();

      foreach (var rightId in rightIds)
      {
        if (rights.Any(r => r == rightId))
        {
          continue;
        }

        return false;
      }

      return true;
    }

    public async Task<List<DbUserRole>> GetAsync(List<Guid> usersIds, string locale)
    {
      IQueryable<DbUserRole> dbUsersRoles = _provider.UsersRoles.AsQueryable();

      dbUsersRoles = dbUsersRoles
        .Include(u => u.Role)
        .ThenInclude(r => r.RoleLocalizations.Where(rl => rl.Locale == locale));

      dbUsersRoles = dbUsersRoles
        .Include(u => u.Role)
        .ThenInclude(r => r.RolesRights);

      return await dbUsersRoles.ToListAsync();
    }

    public async Task<DbUserRole> GetAsync(Guid userId)
    {
      return await _provider.UsersRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);
    }

    public async Task<List<DbUserRole>> GetWithRightsAsync()
    {
      return await _provider.UsersRoles
        .Where(user => user.IsActive)
        .Include(x => x.Role).ThenInclude(x => x.RolesRights)
        .ToListAsync();
    }

    public async Task<bool> RemoveAsync(Guid userId)
    {
      DbUserRole user = _provider.UsersRoles.FirstOrDefault(u => u.UserId == userId && u.IsActive);

      if (user is null)
      {
        return false;
      }

      user.IsActive = false;
      await _provider.SaveAsync();
      return true;
    }
  }
}
