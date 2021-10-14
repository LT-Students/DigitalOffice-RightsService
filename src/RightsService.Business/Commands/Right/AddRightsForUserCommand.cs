﻿using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.Right.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Business.Commands.Right
{
    /// <inheritdoc cref="IAddRightsForUserCommand"/>
    public class AddRightsForUserCommand : IAddRightsForUserCommand
    {
        private readonly IRightLocalizationRepository _repository;
        private readonly IRightsIdsValidator _validator;
        private readonly IAccessValidator _accessValidator;

        public AddRightsForUserCommand(
            IRightLocalizationRepository repository,
            IRightsIdsValidator validator,
            IAccessValidator accessValidator)
        {
            _repository = repository;
            _validator = validator;
            _accessValidator = accessValidator;
        }

        public OperationResultResponse<bool> Execute(Guid userId, IEnumerable<int> rightsIds)
        {
            if (!_accessValidator.IsAdmin())
            {
                throw new ForbiddenException("You need to be an admin to add rights.");
            }

            _validator.ValidateAndThrowCustom(rightsIds);

            _repository.AddRightsToUser(userId, rightsIds);

            return new OperationResultResponse<bool>
            {
                Body = true,
                Status = OperationResultStatusType.FullSuccess
            };
        }
    }
}
