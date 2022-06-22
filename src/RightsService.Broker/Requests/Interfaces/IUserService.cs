using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models;

namespace LT.DigitalOffice.RightsService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IUserService
  {
    Task<List<Guid>> CheckUsersExistence(List<Guid> usersIds, List<string> errors);
    Task<List<UserData>> GetUsersAsync(List<Guid> usersIds, List<string> errors);
  }
}
