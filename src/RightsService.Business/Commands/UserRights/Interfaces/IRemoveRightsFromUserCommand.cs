using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;

namespace LT.DigitalOffice.RightsService.Business.Commands.UserRights.Interfaces
{
  /// <summary>
  /// Represents the command pattern.
  /// Provides method for removing rights from user.
  /// </summary>
  [AutoInject]
  public interface IRemoveRightsFromUserCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(Guid userId, IEnumerable<int> rightsIds);
  }
}
