using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Requests;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.RightsService.Data.Provider;
using MassTransit;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class CheckUserAnyRightConsumer : IConsumer<ICheckUserAnyRightRequest>
  {
    private readonly IDataProvider _provider;

    private object HasAnyRightAsync(ICheckUserAnyRightRequest request)
    {
      return request.RightIds.Intersect(
        from user in _provider.UsersRoles
        where user.UserId == request.UserId && user.IsActive
        join role in _provider.Roles on user.RoleId equals role.Id
        where role.IsActive
        join rolesRights in _provider.RolesRights on role.Id equals rolesRights.RoleId
        select rolesRights.RightId).Any();
    }

    public CheckUserAnyRightConsumer(IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task Consume(ConsumeContext<ICheckUserAnyRightRequest> context)
    {
      var response = OperationResultWrapper.CreateResponse(HasAnyRightAsync, context.Message);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }
  }
}
