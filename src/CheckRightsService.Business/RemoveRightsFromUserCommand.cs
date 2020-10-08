using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Business.Interfaces;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models.Dto;
using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LT.DigitalOffice.CheckRightsService.Business
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
            var validationResult = validator.Validate(request);

            if (validationResult != null && !validationResult.IsValid)
            {
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            if (!accessValidator.IsAdmin())
            {
                throw new ForbiddenException("You need to be an admin to remove rights.");
            }

            repository.RemoveRightsFromUser(request);
        }
    }
}