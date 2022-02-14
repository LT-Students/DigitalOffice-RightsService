using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Models.Right;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using MassTransit;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class FilterRolesUsersConsumer : IConsumer<IFilterRolesRequest>
  {
    private readonly IUserRepository _repository;

    private async Task<List<RoleFilteredData>> GetRolesDataAsync(IFilterRolesRequest request)
    {
      List<DbUser> users = await _repository.GetUsersAsync(request.RolesIds);

      List<DbRole> roles = users.Select(u => u.Role).Distinct().ToList();

      return roles.Select(r =>
          new RoleFilteredData(
            r.Id,
            r.RoleLocalizations.FirstOrDefault()?.Name,
            users.Where(u => u.RoleId == r.Id).Select(u => u.UserId).ToList()))
        .ToList();
    }
    public FilterRolesUsersConsumer(
      IUserRepository repository)
    { 
      _repository = repository;
    }

    public async Task Consume(ConsumeContext<IFilterRolesRequest> context)
    {
      List<RoleFilteredData> rolesFilteredData = await GetRolesDataAsync(context.Message);

      await context.RespondAsync<IOperationResult<IFilterRolesResponse>>(
        OperationResultWrapper.CreateResponse((_) => IFilterRolesResponse.CreateObj(rolesFilteredData), context));

    }
  }
}
