using LT.DigitalOffice.Kernel.AccessValidatorEngine.Requests;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
    public class AccessValidatorConsumer : IConsumer<ICheckUserRightsRequest>
    {
        private readonly IRightRepository repository;

        public AccessValidatorConsumer([FromServices] IRightRepository repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<ICheckUserRightsRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(HasRight, context.Message);
            await context.RespondAsync<IOperationResult<bool>>(response);
        }

        private object HasRight(ICheckUserRightsRequest request)
        {
            if (repository.CheckUserHasRights(request.UserId, request.RightIds))
            {
                return true;
            }

            return false;
        }
    }
}
