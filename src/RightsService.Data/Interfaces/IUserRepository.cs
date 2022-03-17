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

    Task<List<DbUserRole>> GetAsync(List<Guid> userId, string locale);

    Task<DbUserRole> GetAsync(Guid userId);

    Task<List<DbUserRole>> GetWithRightsAsync();

    Task RemoveAsync(Guid userId);
  }
}
