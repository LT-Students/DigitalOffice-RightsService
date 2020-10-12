using LT.DigitalOffice.CheckRightsService.Business.Interfaces;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Business
{
    /// <inheritdoc cref="IAddRightsForUserCommand"/>
    public class AddRightsForUserCommand : IAddRightsForUserCommand
    {
        private readonly ICheckRightsRepository repository;
        private readonly IAccessValidator accessValidator;

        public AddRightsForUserCommand(
            [FromServices] ICheckRightsRepository repository,
            IAccessValidator accessValidator)
        {
            this.repository = repository;
            this.accessValidator = accessValidator;
        }

        public void Execute(Guid userId, IEnumerable<int> rightsIds)
        {
            if (!accessValidator.IsAdmin())
            {
                throw new ForbiddenException("You need to be an admin to add rights.");
            }

            repository.AddRightsToUser(userId, rightsIds);
        }
    }
}