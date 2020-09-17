using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Commands.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models;
using LT.DigitalOffice.CheckRightsService.Repositories.Interfaces;
using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.CheckRightsService.Commands
{
    public class RemoveRightsFromUserCommand : IRemoveRightsFromUserCommand
    {
        private readonly ICheckRightsRepository repository;
        private readonly IValidator<RemoveRightsFromUserRequest> validator;
        private readonly IAccessValidator accessValidator;

        public RemoveRightsFromUserCommand(
            [FromServices] ICheckRightsRepository repository,
            [FromServices] IValidator<RemoveRightsFromUserRequest> validator,
            IAccessValidator accessValidator)
        {
            this.repository = repository;
            this.validator = validator;
            this.accessValidator = accessValidator;
        }

        public void Execute(RemoveRightsFromUserRequest request)
        {
            validator.ValidateAndThrow(request);

            if(!accessValidator.IsAdmin().Result)
            {
                throw new ForbiddenException("You need to be an admin to remove rights.");
            }

            repository.RemoveRightsFromUser(request);
        }
    }
}