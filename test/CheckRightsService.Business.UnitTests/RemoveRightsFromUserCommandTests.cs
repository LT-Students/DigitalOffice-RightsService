using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Commands;
using LT.DigitalOffice.CheckRightsService.Commands.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models;
using LT.DigitalOffice.CheckRightsService.Repositories.Interfaces;
using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.CheckRightsServiceUnitTests.Commands
{
    public class RemoveRightsFromUserCommandTests
    {
        private Mock<ICheckRightsRepository> repositoryMock;
        private Mock<IValidator<RemoveRightsFromUserRequest>> validatorMock;
        private IRemoveRightsFromUserCommand command;
        private Mock<IAccessValidator> accessValidator;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<ICheckRightsRepository>();
            validatorMock = new Mock<IValidator<RemoveRightsFromUserRequest>>();
            accessValidator = new Mock<IAccessValidator>();
            command = new RemoveRightsFromUserCommand(repositoryMock.Object, validatorMock.Object, accessValidator.Object);
        }

        [Test]
        public void ShouldRemoveRightsFromUser()
        {
            var goodRequest = new RemoveRightsFromUserRequest
            {
                UserId = Guid.NewGuid(),
                RightIds = new List<int>() { 0, 1 }
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
                .Setup(x => x.RemoveRightsFromUser(It.IsAny<RemoveRightsFromUserRequest>()));

            command.Execute(goodRequest);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenUserIsNotAdmin()
        {
            var badRequest = new RemoveRightsFromUserRequest
            {
                UserId = Guid.NewGuid(),
                RightIds = new List<int>() { 0, 1 }
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
                .Setup(x => x.RemoveRightsFromUser(It.IsAny<RemoveRightsFromUserRequest>()));

            Assert.Throws<ForbiddenException>(() => command.Execute(badRequest));
            repositoryMock.Verify(repository => repository.RemoveRightsFromUser(It.IsAny<RemoveRightsFromUserRequest>()), Times.Never);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenValidatorThrowException()
        {
            var badRequest = new RemoveRightsFromUserRequest();

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            var task = new Task<bool>(() => true);
            task.RunSynchronously();

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(task);

            repositoryMock
                .Setup(x => x.RemoveRightsFromUser(It.IsAny<RemoveRightsFromUserRequest>()));

            Assert.Throws<ValidationException>(() => command.Execute(badRequest));
            repositoryMock.Verify(repository => repository.RemoveRightsFromUser(It.IsAny<RemoveRightsFromUserRequest>()), Times.Never);
        }
    }
}