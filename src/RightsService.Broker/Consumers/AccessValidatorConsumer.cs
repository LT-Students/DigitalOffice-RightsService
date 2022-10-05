using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Requests;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.RightsService.Data.Provider;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class AccessValidatorConsumer : IConsumer<ICheckUserRightsRequest>
  {
    private readonly IDataProvider _provider;

    private async Task<object> HasRightAsync(ICheckUserRightsRequest request)
    {
      return request.RightIds.Intersect(await
          (from user in _provider.UsersRoles
           where user.UserId == request.UserId && user.IsActive
           join role in _provider.Roles on user.RoleId equals role.Id
           where role.IsActive
           join rolesRights in _provider.RolesRights on role.Id equals rolesRights.RoleId
           select rolesRights.RightId)
          .ToListAsync())
        .Count() == request.RightIds.Count();
    }

    public AccessValidatorConsumer(IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task Consume(ConsumeContext<ICheckUserRightsRequest> context)
    {
      var response = OperationResultWrapper.CreateResponse(HasRightAsync, context.Message);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }
  }
}
