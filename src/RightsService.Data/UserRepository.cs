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
    private readonly IDbUserRightMapper _dbUserRightMapper;

    public UserRepository(
      IDataProvider provider,
      IDbUserMapper dbUserMapper,
      IDbUserRightMapper dbUserRightMapper)
    {
      _provider = provider;
      _dbUserMapper = dbUserMapper;
      _dbUserRightMapper = dbUserRightMapper;
    }

    public async Task AssignRoleAsync(Guid userId, Guid roleId, Guid assignedBy)
    {
      var editedUser = _provider.Users.FirstOrDefault(x => x.UserId == userId);

      if (editedUser != null)
      {
        editedUser.RoleId = roleId;
        _provider.Save();

        return;
      }

      _provider.Users.Add(_dbUserMapper.Map(userId, roleId, assignedBy));

      await _provider.SaveAsync();
    }

    public async Task<bool> CheckRightsAsync(Guid userId, params int[] rightIds)
    {
      if (rightIds == null)
      {
        return false;
      }

      DbUser user = await _provider.Users
        .Include(u => u.Role)
          .ThenInclude(r => r.RoleRights)
        .Include(u => u.Rights)
        .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

      if (user == null)
      {
        return false;
      }

      foreach (var rightId in rightIds)
      {
        if (user.Rights.Any(r => r.RightId == rightId) || user.Role.RoleRights.Any(r => r.RightId == rightId))
        {
          continue;
        }

        return false;
      }

      return true;
    }

    public async Task<List<DbUser>> GetAsync(List<Guid> userId, string locale)
    {
      return await _provider.Users
        .Where(u => userId.Contains(u.UserId))
        .Include(u => u.Role)
        .ThenInclude(r => r.RoleLocalizations.Where(rl => rl.Locale == locale))
        .ToListAsync();
    }

    public async Task<DbUser> GetAsync(Guid userId)
    {
      return await _provider.Users.Include(x => x.Rights).FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<List<DbUser>> GetAsync(List<Guid> rolesIds)
    {
      return await _provider.Users.Include(x => x.Role).Where(
        u => u.IsActive && 
        rolesIds.Contains(u.RoleId.Value))
        .ToListAsync();
    }

    public async Task<List<DbUser>> GetWithRightsAsync()
    {
      return await _provider.Users.Include(x => x.Rights).ToListAsync();
    }

    public async Task RemoveAsync(Guid userId)
    {
      DbUser user = _provider.Users.FirstOrDefault(u => u.UserId == userId);

      if (user == null)
      {
        return;
      }

      user.IsActive = false;
      await _provider.SaveAsync();
    }

    public async Task AddUserRightsAsync(Guid userId, IEnumerable<int> rightIds)
    {
      if (!await _provider.Users.AnyAsync(u => u.IsActive && u.UserId == userId))
      {
        _provider.Users.Add(_dbUserMapper.Map(userId, null));
      }

      List<int> usersRights =
        await _provider.UsersRights.Where(r => userId == r.UserId).Select(r => r.RightId).ToListAsync();

      _provider.UsersRights.AddRange(rightIds.Where(right => !usersRights.Contains(right)).Select(right => _dbUserRightMapper.Map(userId, right)));
      await _provider.SaveAsync();
    }

    public async Task<bool> RemoveUserRightsAsync(Guid userId, IEnumerable<int> rightsIds)
    {
      var userRights = _provider.UsersRights.Where(ru =>
          ru.UserId == userId && rightsIds.Contains(ru.RightId));

      _provider.UsersRights.RemoveRange(userRights);

      await _provider.SaveAsync();

      return true;
    }
  }
}
