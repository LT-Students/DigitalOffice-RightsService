using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;

namespace LT.DigitalOffice.RightsService.Data.Interfaces
{
  [AutoInject]
  public interface IUserRepository
  {
    Task AssignRoleAsync(Guid userId, Guid roleId, Guid assignedBy);

    Task<bool> CheckRightsAsync(Guid userId, params int[] rightIds);

    Task<List<DbUser>> GetAsync(List<Guid> userId, string locale);

    Task RemoveAsync(Guid userId);

    Task AddUserRightsAsync(Guid userId, IEnumerable<int> rightIds);

    Task<bool> RemoveUserRightsAsync(Guid userId, IEnumerable<int> rightIds);
  }
}
