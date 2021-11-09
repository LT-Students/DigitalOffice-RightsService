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
  public class UserRepository : IUserRepository
  {
    private readonly IDataProvider _provider;

    public UserRepository(IDataProvider provider)
    {
      _provider = provider;
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

      _provider.Users.Add(
        new DbUser
        {
          Id = Guid.NewGuid(),
          UserId = userId,
          RoleId = roleId,
          CreatedAtUtc = DateTime.UtcNow,
          CreatedBy = assignedBy,
          IsActive = true
        });

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
        .FirstOrDefaultAsync(u => u.UserId == userId);

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

    public async Task AddUserRightsAsync(Guid userId, IEnumerable<int> rightsIds)
    {
      foreach (var rightId in rightsIds)
      {
        //TODO rework
        var dbRight = _provider.RightsLocalizations.FirstOrDefault(right => right.RightId == rightId);

        if (dbRight == null)
        {
          continue;
        }

        var dbRightUser = _provider.UsersRights.FirstOrDefault(rightUser =>
            rightUser.RightId == rightId && rightUser.UserId == userId);

        if (dbRightUser == null)
        {
          _provider.UsersRights.Add(new DbUserRight
          {
            UserId = userId,
            RightId = rightId,
          });
        }
      }
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
