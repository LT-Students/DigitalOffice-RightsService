using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.RightsService.Business.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Business
{
    /// <inheritdoc cref="IRemoveRightsFromUserCommand"/>
    public class RemoveRightsFromUserCommand : IRemoveRightsFromUserCommand
    {
        private readonly ICheckRightsRepository repository;
        private readonly IValidator<IEnumerable<int>> validator;
        private readonly IAccessValidator accessValidator;

        public RemoveRightsFromUserCommand(
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
                throw new ForbiddenException("You need to be an admin to remove rights.");
            }

            validator.ValidateAndThrowCustom(rightsIds);

            repository.RemoveRightsFromUser(userId, rightsIds);
        }
    }
}