using FluentValidation;
using LT.DigitalOffice.RightsService.Business.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Exceptions.Models;

namespace LT.DigitalOffice.RightsService.Business
{
    /// <inheritdoc cref="IAddRightsForUserCommand"/>
    public class AddRightsForUserCommand : IAddRightsForUserCommand
    {
        private readonly ICheckRightsRepository repository;
        private readonly IValidator<IEnumerable<int>> validator;
        private readonly IAccessValidator accessValidator;

        public AddRightsForUserCommand(
            ICheckRightsRepository repository,
            IValidator<IEnumerable<int>> validator,
            IAccessValidator accessValidator)
        {
            this.repository = repository;
            this.validator = validator;
            this.accessValidator = accessValidator;
        }

        public void Execute(Guid userId, IEnumerable<int> rightsIds)
        {
            if (!accessValidator.IsAdmin())
            {
                throw new ForbiddenException("You need to be an admin to add rights.");
            }

            validator.ValidateAndThrowCustom(rightsIds);

            repository.AddRightsToUser(userId, rightsIds);
        }
    }
}