using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests.Filters;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.RightsService.Data
{
  public class RoleRepository : IRoleRepository
  {
    private readonly IDataProvider _provider;

    public RoleRepository(
      IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task<Guid> CreateAsync(DbRole dbRole)
    {
      _provider.Roles.Add(dbRole);
      await _provider.SaveAsync();

      return dbRole.Id;
    }

    public async Task<(DbRole role, List<DbUser> users, List<DbRightsLocalization> rights)> GetAsync(GetRoleFilter filter)
    {
      return (await
        (from role in _provider.Roles
         join roleLocalization in _provider.RolesLocalizations on role.Id equals roleLocalization.RoleId
         join right in _provider.RoleRights on role.Id equals right.RoleId
         join rightLocalization in _provider.RightsLocalizations on right.RightId equals rightLocalization.RightId
         join user in _provider.Users on role.Id equals user.RoleId into Users
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

    public async Task<(List<(DbRole role, List<DbRightsLocalization> rights)>, int totalCount)> FindAsync(FindRolesFilter filter)
    {
      int totalCount = await _provider.Roles.CountAsync();

      return ((await
        (from role in _provider.Roles
          join roleLocalization in _provider.RolesLocalizations on role.Id equals roleLocalization.RoleId
          join right in _provider.RoleRights on role.Id equals right.RoleId
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
          }).ToList(),
        totalCount);
    }

    public async Task<bool> DoesRoleExistAsync(Guid roleId)
    {
      return await _provider.Roles.AnyAsync(r => r.Id == roleId);
    }
  }
}
