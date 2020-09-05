using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Commands;
using LT.DigitalOffice.CheckRightsService.Commands.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models;
using LT.DigitalOffice.CheckRightsService.Repositories.Interfaces;
using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using Moq;
using NUnit.Framework;

namespace LT.DigitalOffice.CheckRightsServiceUnitTests.Commands
{
    public class AddRightsForUserCommandTests
    {
        private Mock<ICheckRightsRepository> repositoryMock;
        private Mock<IValidator<AddRightsForUserRequest>> validatorMock;
        private Mock<IAccessValidator> accessValidator;
        private IAddRightsForUserCommand command;

        [SetUp]
        public void Setup()
        {
            repositoryMock = new Mock<ICheckRightsRepository>();
            validatorMock = new Mock<IValidator<AddRightsForUserRequest>>();
            accessValidator = new Mock<IAccessValidator>();
            command = new AddRightsForUserCommand(repositoryMock.Object, validatorMock.Object, accessValidator.Object);
        }

        [Test]
        public void ShouldAddRightsForUser()
        {
            var request = new AddRightsForUserRequest
            {
                UserId = Guid.NewGuid(),
                RightsIds = new List<int>() { 0, 1 }
            };

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            var task = new Task<bool>(() => true);
            task.RunSynchronously();

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(task);

            repositoryMock
                .Setup(x => x.AddRightsToUser(It.IsAny<AddRightsForUserRequest>()));

            command.Execute(request);
        }

        [Test]
        public void ShouldThrowForbiddenExceptionWhenAccessValidatorThrowFalse()
        {
            var request = new AddRightsForUserRequest
            {
                UserId = Guid.NewGuid(),
                RightsIds = new List<int>() { 0, 1 }
            };

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            var task = new Task<bool>(() => false);
            task.RunSynchronously();

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(task);

            repositoryMock
                .Setup(x => x.AddRightsToUser(It.IsAny<AddRightsForUserRequest>()));

            Assert.Throws<ForbiddenException>(() => command.Execute(request));
        }

        [Test]
        public void ShouldThrowValidationExceptionWheValidatorThrowException()
        {
            var request = new AddRightsForUserRequest
            {
                RightsIds = new List<int>() { 0, 1 }
            };

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            var task = new Task<bool>(() => true);
            task.RunSynchronously();

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(task);

            Assert.Throws<ValidationException>(() => command.Execute(request));
            repositoryMock.Verify(repository => repository.AddRightsToUser(It.IsAny<AddRightsForUserRequest>()), Times.Never);
        }

        [Test]
        public void ShouldThrowBadRequestExceptionWhenRepositoryThrowException()
        {
            var request = new AddRightsForUserRequest
            {
                RightsIds = new List<int>() { 1 }
            };

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            var task = new Task<bool>(() => true);
            task.RunSynchronously();

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(task);

            repositoryMock
                .Setup(x => x.AddRightsToUser(It.IsAny<AddRightsForUserRequest>()))
                .Throws(new BadRequestException());

            Assert.Throws<BadRequestException>(() => command.Execute(request));
        }
    }
}