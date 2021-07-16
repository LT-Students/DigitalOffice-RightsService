using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using MassTransit;
using System.Threading.Tasks;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
    public class ChangeUserRoleConsumer : IConsumer<IChangeUserRoleRequest>
    {
        private readonly IUserRepository _userRepository;

        private object ChangeRole(IChangeUserRoleRequest request)
        {
            _userRepository.AssignRole(request.UserId, request.RoleId, request.ChangedBy);

            return true;
        }

        public ChangeUserRoleConsumer(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<IChangeUserRoleRequest> context)
        {
            var result = OperationResultWrapper.CreateResponse(ChangeRole, context.Message);

            await context.RespondAsync<IOperationResult<bool>>(result);
        }
    }
}
