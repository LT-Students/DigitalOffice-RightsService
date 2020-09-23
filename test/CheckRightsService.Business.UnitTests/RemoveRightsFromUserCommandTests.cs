using FluentValidation;
using FluentValidation.Results;
using LT.DigitalOffice.CheckRightsService.Business.Interfaces;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models.Dto;
using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Business.UnitTests
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

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(true);

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

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(false);

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
                .Setup(x => x.Validate(It.IsAny<RemoveRightsFromUserRequest>()))
                .Returns(new ValidationResult(
                    new List<ValidationFailure>
                    {
                        new ValidationFailure("test", "something", null)
                    }));

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(true);

            repositoryMock
                .Setup(x => x.RemoveRightsFromUser(It.IsAny<RemoveRightsFromUserRequest>()));

            Assert.Throws<ValidationException>(() => command.Execute(badRequest));
            repositoryMock.Verify(repository => repository.RemoveRightsFromUser(It.IsAny<RemoveRightsFromUserRequest>()), Times.Never);
        }
    }
}