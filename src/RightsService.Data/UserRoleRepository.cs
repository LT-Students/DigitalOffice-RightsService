using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalOffice.Models.Broker.Publishing;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.RightsService.Data
{
  public class UserRoleRepository : IUserRoleRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserRoleRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
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

    public async Task<Guid?> ActivateAsync(IActivateUserPublish request)
    {
      DbUserRole user = await _provider.UsersRoles.FirstOrDefaultAsync(u => u.UserId == request.UserId && !u.IsActive);

      if (user is null)
      {
        return null;
      }

      user.IsActive = true;
      await _provider.SaveAsync();

      return user.RoleId;
    }

    public async Task<bool> EditAsync(DbUserRole oldUser, Guid roleId)
    {
      if (oldUser is null)
      {
        return false;
      }

      oldUser.RoleId = roleId;
      oldUser.IsActive = true;

      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> CheckRightsAsync(Guid userId, params int[] rightIds)
    {
      if (rightIds is null)
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

    public Task<List<DbUserRole>> GetAsync(List<Guid> usersIds, string locale)
    {
      if (usersIds is null || !usersIds.Any())
      {
        return null;
      }

      IQueryable<DbUserRole> dbUsersRoles = _provider.UsersRoles.AsQueryable();

      dbUsersRoles = dbUsersRoles
        .Include(u => u.Role)
        .ThenInclude(r => r.RoleLocalizations.Where(rl => rl.Locale == locale));

      dbUsersRoles = dbUsersRoles
        .Include(u => u.Role)
        .ThenInclude(r => r.RolesRights);

      return dbUsersRoles.Where(ur => usersIds.Contains(ur.UserId) && ur.IsActive).ToListAsync();
    }

    public Task<DbUserRole> GetAsync(Guid userId)
    {
      return _provider.UsersRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);
    }

    public Task<List<DbUserRole>> GetWithRightsAsync()
    {
      return _provider.UsersRoles
        .Where(user => user.IsActive)
        .Include(x => x.Role).ThenInclude(x => x.RolesRights)
        .ToListAsync();
    }

    public Task<List<DbUserRole>> GetWithoutRightsAsync()
    {
      return _provider.UsersRoles
        .Where(user => user.IsActive)
        .ToListAsync();
    }

    public async Task<bool> RemoveAsync(Guid userId, DbUserRole removedUser = null, Guid? removedBy = null)
    {
      DbUserRole user = removedUser
        ?? await _provider.UsersRoles.FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

      if (user is null)
      {
        return false;
      }

      user.IsActive = false;
      user.CreatedBy = removedBy.HasValue
        ? removedBy.Value
        : _httpContextAccessor.HttpContext.GetUserId();

      await _provider.SaveAsync();
      return true;
    }

    public Task<bool> DoesExistAsync(Guid userId)
    {
      return _provider.UsersRoles.AnyAsync(x => x.UserId == userId && x.IsActive);
    }
  }
}
