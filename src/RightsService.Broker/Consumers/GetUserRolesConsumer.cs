using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using MassTransit;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class GetUserRolesConsumer : IConsumer<IGetUserRolesRequest>
  {
    private readonly IUserRepository _repository;

    private async Task<object> GetRolesAsync(IGetUserRolesRequest request)
    {
      List<DbUser> users = await _repository.GetAsync(request.UserIds, request.Locale);

      List<DbRole> roles = users.Select(u => u.Role).Distinct().ToList();

      return IGetUserRolesResponse.CreateObj(
        roles.Select(r =>
          new RoleData(
            r.Id,
            r.RoleLocalizations.FirstOrDefault()?.Name,
            r.RoleLocalizations.FirstOrDefault()?.Description,
            users.Where(u => u.RoleId == r.Id).Select(u => u.UserId).ToList()))
        .ToList());
    }

    public GetUserRolesConsumer(IUserRepository userRepository)
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
