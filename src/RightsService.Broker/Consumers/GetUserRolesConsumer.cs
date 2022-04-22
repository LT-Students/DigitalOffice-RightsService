using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Right;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using MassTransit;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class GetUserRolesConsumer : IConsumer<IGetUserRolesRequest>
  {
    private readonly IUserRoleRepository _repository;

    private async Task<object> GetRolesAsync(IGetUserRolesRequest request)
    {
      List<DbUserRole> dbUsersRoles = await _repository.GetAsync(request.UserIds, request.Locale);

      List<DbRole> dbRoles = dbUsersRoles.Select(u => u.Role).Distinct().ToList();

      return IGetUserRolesResponse.CreateObj(
        dbRoles.Select(r =>
          new RoleData(
            r.Id,
            r.RoleLocalizations.FirstOrDefault()?.Name,
            r.RolesRights.Select(rr => rr.RightId).ToList(),
            dbUsersRoles.Where(u => u.RoleId == r.Id).Select(u => u.UserId).ToList()))
        .ToList());
    }

    public GetUserRolesConsumer(IUserRoleRepository userRepository)
    {
      _repository = userRepository;
    }

    public async Task Consume(ConsumeContext<IGetUserRolesRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(GetRolesAsync, context.Message);

      await context.RespondAsync<IOperationResult<IGetUserRolesResponse>>(response);
    }
  }
}
