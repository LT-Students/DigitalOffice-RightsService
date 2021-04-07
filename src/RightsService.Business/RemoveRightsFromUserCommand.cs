using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.RightsService.Business.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Business
{
    /// <inheritdoc cref="IRemoveRightsFromUserCommand"/>
    public class RemoveRightsFromUserCommand : IRemoveRightsFromUserCommand
    {
        private readonly ICheckRightsRepository _repository;
        private readonly IRightsIdsValidator _validator;
        private readonly IAccessValidator _accessValidator;

        public RemoveRightsFromUserCommand(
            ICheckRightsRepository repository,
            IRightsIdsValidator validator,
            IAccessValidator accessValidator)
        {
            _repository = repository;
            _validator = validator;
            _accessValidator = accessValidator;
        }

        public void Execute(Guid userId, IEnumerable<int> rightsIds)
        {
            if (!_accessValidator.IsAdmin())
            {
                throw new ForbiddenException("You need to be an admin to remove rights.");
            }

            _validator.ValidateAndThrowCustom(rightsIds);

            _repository.RemoveRightsFromUser(userId, rightsIds);
        }
    }
}
