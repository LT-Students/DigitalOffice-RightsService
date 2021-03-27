using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Requests;
using LT.DigitalOffice.Kernel.Broker;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.CheckRightsService.Broker.Consumers
{
    public class AccessCollectionValidatorConsumer : IConsumer<IAccessValidatorCheckRightsCollectionServiceRequest>
    {
        private readonly ICheckRightsRepository _repository;

        public AccessCollectionValidatorConsumer([FromServices] ICheckRightsRepository repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<IAccessValidatorCheckRightsCollectionServiceRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(HasRights, context.Message);
            await context.RespondAsync<IOperationResult<bool>>(response);
        }

        private object HasRights(IAccessValidatorCheckRightsCollectionServiceRequest request)
        {
            foreach(var rigthId in request.RightIds)
            {
                if (!_repository.IsUserHasRight(request.UserId, rigthId))
                {
                    throw new Exception("Such user doesn't exist or does not have this rights.");
                }
            }

            return true;
        }
    }
}
