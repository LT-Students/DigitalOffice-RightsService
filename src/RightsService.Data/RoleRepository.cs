﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.RightsService.Data
{
  public class RoleRepository : IRoleRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoleRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid> CreateAsync(DbRole dbRole)
    {
      _provider.Roles.Add(dbRole);
      await _provider.SaveAsync();

      return dbRole.Id;
    }

    public async Task<(DbRole role, List<DbUserRole> users, List<DbRightLocalization> rights)> GetAsync(GetRoleFilter filter)
    {
      return (await
        (from role in _provider.Roles
         join roleLocalization in _provider.RolesLocalizations on role.Id equals roleLocalization.RoleId
         join right in _provider.RolesRights on role.Id equals right.RoleId
         join rightLocalization in _provider.RightsLocalizations on right.RightId equals rightLocalization.RightId
         join user in _provider.UsersRoles on role.Id equals user.RoleId into Users
         from user in Users.DefaultIfEmpty()
         where role.Id == filter.RoleId
           && (roleLocalization.Locale == filter.Locale || roleLocalization.Locale == null)
           && (rightLocalization.Locale == filter.Locale || rightLocalization.Locale == null)
         select new
         {
           Role = role,
           RoleLocalization = roleLocalization,
           RightLocalization = rightLocalization,
           User = user
         }).ToListAsync()).AsEnumerable().GroupBy(r => r.Role.Id)
         .Select(x =>
         {
           DbRole role = x.Select(x => x.Role).FirstOrDefault();
           role.RoleLocalizations = x.Select(x => x.RoleLocalization).Where(x => x != null).GroupBy(x => x.Id).Select(x => x.First()).ToList();

           return (role, x.Select(x => x.User).Where(u => u != null).ToList(), x.Select(x => x.RightLocalization).ToList());
         }).FirstOrDefault();
    }

    public async Task<DbRole> GetAsync(Guid roleId)
    {
      return await _provider.Roles.Include(role => role.RolesRights).FirstOrDefaultAsync(x => x.Id == roleId);
    }

    public async Task<List<DbRole>> GetAsync(List<Guid> rolesIds)
    {
      return await _provider.Roles.Where(r => rolesIds.Contains(r.Id))
        .Include(r => r.Users.Where(u => u.IsActive))
        .ToListAsync();
    }

    public async Task<List<DbRole>> GetAllWithRightsAsync()
    {
      return await _provider.Roles.Include(role => role.RolesRights).ToListAsync();
    }

    public async Task<(List<(DbRole role, List<DbRightLocalization> rights)>, int totalCount)> FindAllAsync(FindRolesFilter filter)
    {
      int totalCount = await _provider.Roles.CountAsync();

      return ((await
        (from role in _provider.Roles
          join roleLocalization in _provider.RolesLocalizations on role.Id equals roleLocalization.RoleId where roleLocalization.IsActive
          join right in _provider.RolesRights on role.Id equals right.RoleId
          join rightLocalization in _provider.RightsLocalizations on right.RightId equals rightLocalization.RightId
          where (roleLocalization.Locale == filter.Locale || roleLocalization.Locale == null)
            && (rightLocalization.Locale == filter.Locale || rightLocalization.Locale == null)
          orderby role.Id
          select new
          {
            Role = role,
            RoleLocalization = roleLocalization,
            RightLocalization = rightLocalization
          }).ToListAsync()).AsEnumerable().GroupBy(r => r.Role.Id)
          .Select(x =>
          {
            DbRole role = x.Select(x => x.Role).FirstOrDefault();
            role.RoleLocalizations = x.Select(x => x.RoleLocalization).Where(x => x != null).GroupBy(x => x.Id).Select(x => x.First()).ToList();

            return (role, x.Select(x => x.RightLocalization).ToList());
          }).Skip(filter.SkipCount).Take(filter.TakeCount).ToList(),
        totalCount);
    }

    public async Task<(List<(DbRole role, List<DbRightLocalization> rights)>, int totalCount)> FindActiveAsync(FindRolesFilter filter)
    {
      int totalCount = await _provider.Roles.CountAsync(x => x.IsActive);

      return ((await
        (from role in _provider.Roles where role.IsActive
         join roleLocalization in _provider.RolesLocalizations on role.Id equals roleLocalization.RoleId where roleLocalization.IsActive
         join right in _provider.RolesRights on role.Id equals right.RoleId
         join rightLocalization in _provider.RightsLocalizations on right.RightId equals rightLocalization.RightId
         where (roleLocalization.Locale == filter.Locale || roleLocalization.Locale == null)
           && (rightLocalization.Locale == filter.Locale || rightLocalization.Locale == null)
         orderby role.Id
         select new
         {
           Role = role,
           RoleLocalization = roleLocalization,
           RightLocalization = rightLocalization
         }).ToListAsync()).AsEnumerable().GroupBy(r => r.Role.Id)
          .Select(x =>
          {
            DbRole role = x.Select(x => x.Role).FirstOrDefault();
            role.RoleLocalizations = x.Select(x => x.RoleLocalization).Where(x => x != null).GroupBy(x => x.Id).Select(x => x.First()).ToList();

            return (role, x.Select(x => x.RightLocalization).ToList());
          }).Skip(filter.SkipCount).Take(filter.TakeCount).ToList(),
        totalCount);
    }

    public async Task<bool> DoesRoleExistAsync(Guid roleId)
    {
      return await _provider.Roles.AnyAsync(r => r.Id == roleId);
    }

    public async Task<bool> EditStatusAsync(Guid roleId, bool isActive)
    {
      DbRole role = _provider.Roles.FirstOrDefault(x => x.Id == roleId);

      if (role == null)
      {
        return false;
      }

      _provider.Roles.Update(role);

      role.IsActive = isActive;
      role.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      role.ModifiedAtUtc = DateTime.UtcNow;
      
      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> EditRoleRightsAsync(Guid roleId, List<DbRoleRight> newRights)
    {
      List<DbRoleRight> roleRights = await _provider.RolesRights.Where(x => x.RoleId == roleId).ToListAsync();

      List<int> oldRightsIds =
        (from oldRightIds in roleRights.Select(x => x.RightId).Intersect(newRights.Select(x => x.RightId))
          select oldRightIds)
          .ToList();

      _provider.RolesRights.RemoveRange(roleRights.Where(x => !oldRightsIds.Contains(x.RightId)));
      _provider.RolesRights.AddRange(newRights.Where(x => !oldRightsIds.Contains(x.RightId)));

      await _provider.SaveAsync();

      return true;
    }
  }
}
