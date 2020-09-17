using LT.DigitalOffice.CheckRightsService.Repositories.Interfaces;
using LT.DigitalOffice.Kernel.AccessValidator.Requests;
using LT.DigitalOffice.Kernel.Broker;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LT.DigitalOffice.CheckRightsService.Broker.Consumers
{
    public class AccessValidatorConsumer : IConsumer<IAccessValidatorRequest>
    {
        private readonly ICheckRightsRepository repository;

        public AccessValidatorConsumer([FromServices] ICheckRightsRepository repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<IAccessValidatorRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(HasRights, context.Message);
            await context.RespondAsync<IOperationResult<bool>>(response);
        }

        private object HasRights(IAccessValidatorRequest request)
        {
            return repository.CheckIfUserHasRight(request.UserId, request.RightId);
        }
    }
}
