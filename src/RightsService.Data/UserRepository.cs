using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.RightsService.Data
{
  public class UserRepository : IUserRepository
  {
    private readonly IDataProvider _provider;
    private readonly IDbUserMapper _dbUserMapper;

    public UserRepository(
      IDataProvider provider,
      IDbUserMapper dbUserMapper)
    {
      _provider = provider;
      _dbUserMapper = dbUserMapper;
    }

    public async Task AssignRoleAsync(Guid userId, Guid roleId, Guid assignedBy)
    {
      var editedUser = _provider.UsersRoles.FirstOrDefault(x => x.UserId == userId);

      if (editedUser != null)
      {
        editedUser.RoleId = roleId;
        _provider.Save();

        return;
      }

      _provider.UsersRoles.Add(_dbUserMapper.Map(userId, roleId, assignedBy));

      await _provider.SaveAsync();
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
         join role in _provider.Roles on user.RoleId equals role.Id where role.IsActive
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

    public async Task<List<DbUserRole>> GetAsync(List<Guid> userId, string locale)
    {
      return await _provider.UsersRoles
        .Where(u => userId.Contains(u.UserId))
        .Include(u => u.Role)
        .ThenInclude(r => r.RoleLocalizations.Where(rl => rl.Locale == locale))
        .ToListAsync();
    }

    public async Task<DbUserRole> GetAsync(Guid userId)
    {
      return await _provider.UsersRoles.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<List<DbUserRole>> GetWithRightsAsync()
    {
      return await _provider.UsersRoles
        .Include(x => x.Role).ThenInclude(x => x.RolesRights)
        .ToListAsync();
    }

    public async Task RemoveAsync(Guid userId)
    {
      DbUserRole user = _provider.UsersRoles.FirstOrDefault(u => u.UserId == userId);

      if (user is null)
      {
        return;
      }

      user.IsActive = false;
      await _provider.SaveAsync();
    }
  }
}
