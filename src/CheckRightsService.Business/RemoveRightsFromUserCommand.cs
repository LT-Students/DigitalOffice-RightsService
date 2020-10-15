﻿using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Business.Interfaces;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models.Dto;
using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Business
{
    /// <inheritdoc cref="IRemoveRightsFromUserCommand"/>
    public class RemoveRightsFromUserCommand : IRemoveRightsFromUserCommand
    {
        private readonly ICheckRightsRepository repository;
        private readonly IValidator<IEnumerable<int>> validator;
        private readonly IAccessValidator accessValidator;

        public RemoveRightsFromUserCommand(
            [FromServices] ICheckRightsRepository repository,
            [FromServices] IValidator<IEnumerable<int>> validator,
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