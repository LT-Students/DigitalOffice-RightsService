using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Commands.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models;
using LT.DigitalOffice.CheckRightsService.Repositories.Interfaces;
using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
            var validationResult = validator.Validate(request);

            if (validationResult != null && !validationResult.IsValid)
            {
                var messages = validationResult.Errors.Select(x => x.ErrorMessage);
                string message = messages.Aggregate((x, y) => x + "\n" + y);

                throw new ValidationException(message);
            }

            if (!accessValidator.IsAdmin())
            {
                throw new ForbiddenException("You need to be an admin to add rights.");
            }

            repository.AddRightsToUser(request);
        }
    }
}