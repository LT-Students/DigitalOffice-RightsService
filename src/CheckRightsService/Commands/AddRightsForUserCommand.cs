using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Commands.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models;
using LT.DigitalOffice.CheckRightsService.Repositories.Interfaces;
using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.CheckRightsService.Commands
{
    public class AddRightsForUserCommand : IAddRightsForUserCommand
    {
        private readonly ICheckRightsRepository repository;
        private readonly IValidator<AddRightsForUserRequest> validator;
        private readonly IAccessValidator accessValidator;

        public AddRightsForUserCommand(
            [FromServices] ICheckRightsRepository repository,
            [FromServices] IValidator<AddRightsForUserRequest> validator,
            IAccessValidator accessValidator)
        {
            this.repository = repository;
            this.validator = validator;
            this.accessValidator = accessValidator;
        }

        public void Execute(AddRightsForUserRequest request)
        {
            validator.ValidateAndThrow(request);

            if (!accessValidator.IsAdmin().Result)
            {
                throw new ForbiddenException("You need to be an admin to add rights.");
            }

            repository.AddRightsToUser(request);
        }
    }
}