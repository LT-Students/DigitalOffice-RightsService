using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests.Filters;

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

    public Guid Create(DbRole dbRole)
    {
      _provider.Roles.Add(dbRole);
      _provider.Save();

      return dbRole.Id;
    }

    public (DbRole role, List<DbUser> users, List<DbRightsLocalization> rights) Get(GetRoleFilter filter)
    {
      return
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
         }).AsEnumerable().GroupBy(r => r.Role.Id).Select(x =>
         {
           DbRole role = x.Select(x => x.Role).FirstOrDefault();
           role.RoleLocalizations = x.Select(x => x.RoleLocalization).Where(x => x != null).GroupBy(x => x.Id).Select(x => x.First()).ToList();

           return (role, x.Select(x => x.User).Where(u => u != null).ToList(), x.Select(x => x.RightLocalization).ToList());
         }).FirstOrDefault();
    }

    public List<(DbRole role, List<DbRightsLocalization> rights)> Find(FindRolesFilter filter, out int totalCount)
    {
      totalCount = _provider.Roles.Count();

      return
        (from role in _provider.Roles
         join roleLocalization in _provider.RolesLocalizations on role.Id equals roleLocalization.RoleId
         join right in _provider.RoleRights on role.Id equals right.RoleId
         join rightLocalization in _provider.RightsLocalizations on right.RightId equals rightLocalization.RightId
         where (roleLocalization.Locale == filter.Locale || roleLocalization.Locale == null)
           && (rightLocalization.Locale == filter.Locale || rightLocalization.Locale == null)
         select new
         {
           Role = role,
           RoleLocalization = roleLocalization,
           RightLocalization = rightLocalization
         }).AsEnumerable().GroupBy(r => r.Role.Id).Select(x =>
         {
           DbRole role = x.Select(x => x.Role).FirstOrDefault();
           role.RoleLocalizations = x.Select(x => x.RoleLocalization).Where(x => x != null).GroupBy(x => x.Id).Select(x => x.First()).ToList();

           return (role, x.Select(x => x.RightLocalization).ToList());
         }).ToList();
    }

    public bool DoesRoleExist(Guid roleId)
    {
      return _provider.Roles.Any(r => r.Id == roleId);
    }
  }
}
