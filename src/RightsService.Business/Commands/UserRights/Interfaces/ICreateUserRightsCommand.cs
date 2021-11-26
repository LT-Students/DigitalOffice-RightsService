using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;

namespace LT.DigitalOffice.RightsService.Business.Commands.UserRights.Interfaces
{
  /// <summary>
  /// Represents the command pattern.
  /// Provides a method to add rights for user.
  /// </summary>
  [AutoInject]
  public interface ICreateUserRightsCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(Guid userId, IEnumerable<int> rightsIds);
  }
}
