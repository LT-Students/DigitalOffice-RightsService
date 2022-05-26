using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;

namespace LT.DigitalOffice.RightsService.Data.Interfaces
{
  [AutoInject]
  public interface IUserRoleRepository
  {
    Task<Guid?> CreateAsync(DbUserRole dbUserRole);

    Task<bool> EditAsync(DbUserRole oldUser, EditUserRoleRequest request);

    Task<bool> CheckRightsAsync(Guid userId, params int[] rightIds);

    Task<List<DbUserRole>> GetAsync(List<Guid> usersIds, string locale);

    Task<DbUserRole> GetAsync(Guid userId);

    Task<List<DbUserRole>> GetWithRightsAsync();

    Task<bool> RemoveAsync(Guid userId, DbUserRole removedUser = null, Guid? modifiedBy = null);

    Task<bool> DoesExistAsync(Guid userId);
  }
}
